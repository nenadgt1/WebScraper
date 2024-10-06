document.addEventListener("DOMContentLoaded", () => {
    const makeFilter = document.getElementById('make-filter');
    const yearFilter = document.getElementById('year-filter');
    const priceFilter = document.getElementById('price-filter');
    const filterBtn = document.getElementById('filter-btn');

    // Fetch car data
    fetch('api/cars')
        .then(response => response.json())
        .then(data => {
            console.log(data); // Log the API response to inspect its structure
            populateMakeFilter(data); // Populate the 'Make' filter
            displayCars(data); // Display all cars initially

            // Event listener for filtering
            filterBtn.addEventListener('click', () => {
                const filteredCars = filterCars(data); // Filter based on user input
                displayCars(filteredCars); // Display the filtered cars
            });
        })
        .catch(error => console.error('Error fetching data:', error));

    // Populate 'Make' filter with unique car makes
    function populateMakeFilter(cars) {
        const uniqueMakes = [...new Set(cars.map(car => car.make))]; // Get unique car makes
        uniqueMakes.forEach(make => {
            const option = document.createElement('option');
            option.value = make;
            option.textContent = make;
            makeFilter.appendChild(option); // Add each make as an option in the filter
        });
    }

    // Filter car data based on selected filters
    function filterCars(cars) {
        const selectedMake = makeFilter.value;
        const selectedYear = parseInt(yearFilter.value) || null;
        const maxPrice = parseInt(priceFilter.value) || null;

        return cars.filter(car => {
            const meetsMake = selectedMake === '' || car.make === selectedMake;
            const meetsYear = !selectedYear || car.year === selectedYear;
            const meetsPrice = !maxPrice || parseInt(car.price.replace(/[^\d]/g, '')) <= maxPrice;
            return meetsMake && meetsYear && meetsPrice;
        });
    }

    // Display the list of cars in card format
    function displayCars(cars) {
        const carList = document.getElementById('car-list');
        carList.innerHTML = ''; // Clear current car display

        cars.forEach(car => {
            console.log('Displaying car:', car);  // Log each car object to ensure data is correct
            const carItem = document.createElement('div');
            carItem.classList.add('car-card');
            carItem.innerHTML = `
                <p><strong>Make:</strong> ${car.make || 'N/A'}</p>
                <p><strong>Model:</strong> ${car.model || 'N/A'}</p>
                <p><strong>Year:</strong> ${car.year || 'N/A'}</p>
                <p><strong>Mileage:</strong> ${car.mileage || 'N/A'}</p>
                <p class="price"><strong>Price:</strong> ${car.price ? car.price.replace(/[^\d]/g, '') : 'N/A'} €</p>
                <p class="ad-number"><strong>Ad Number:</strong> <a href="${car.ad_number}" target="_blank">${car.ad_number}</a></p>
            `;
            carList.appendChild(carItem); // Append each car to the car list
        });
    }
});
