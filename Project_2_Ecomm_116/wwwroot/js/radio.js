// wwwroot/js/data-script.js

document.addEventListener('DOMContentLoaded', function () {
    // Get radio buttons and data containers
    const monthlyRadio = document.querySelector('input[value="monthly"]');
    const yearlyRadio = document.querySelector('input[value="yearly"]');
    const monthlyDataContainer = document.getElementById('monthlyData');
    const yearlyDataContainer = document.getElementById('yearlyData');

    // Function to handle radio button change
    function handleRadioChange() {
        if (monthlyRadio.checked) {
            // You can fetch and display the monthly data here using an AJAX call if needed.
            monthlyDataContainer.innerHTML = '<p>Monthly Data: Fetch and display monthly data here.</p>';
            monthlyDataContainer.style.display = 'block';
            yearlyDataContainer.style.display = 'none';
        } else if (yearlyRadio.checked) {
            // You can fetch and display the yearly data here using an AJAX call if needed.
            yearlyDataContainer.innerHTML = '<p>Yearly Data: Fetch and display yearly data here.</p>';
            monthlyDataContainer.style.display = 'none';
            yearlyDataContainer.style.display = 'block';
        }
    }

    // Add event listener to radio buttons
    monthlyRadio.addEventListener('change', handleRadioChange);
    yearlyRadio.addEventListener('change', handleRadioChange);

    // Initial setup
    handleRadioChange();
});
