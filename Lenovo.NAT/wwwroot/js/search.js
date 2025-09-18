function debounce(func, wait = 300) {
    let timeout;
    return function (...args) {
        clearTimeout(timeout);
        timeout = setTimeout(() => func.apply(this, args), wait);
    };
}

document.addEventListener('DOMContentLoaded', function () {
    const searchInput = document.getElementById('searchInput');
    const noResultsMessage = document.getElementById('noResultsMessage');

    searchInput.addEventListener('input', debounce(function () {
        const filter = searchInput.value.toLowerCase();
        let hasResults = false;

        document.querySelectorAll('.card').forEach(card => {
            const title = card.querySelector('.card-title').textContent.toLowerCase();
            const items = Array.from(card.querySelectorAll('.list-group-item a')).map(a => a.textContent.toLowerCase());
            const match = title.includes(filter) || items.some(i => i.includes(filter));
            card.style.display = match ? 'block' : 'none';
            if (match) hasResults = true;
        });

        noResultsMessage.style.display = hasResults ? 'none' : 'block';
    }, 300));
});
