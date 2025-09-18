document.getElementById('toggleTheme').addEventListener('click', function () {
    document.body.classList.toggle('dark-mode');
    this.innerHTML = document.body.classList.contains('dark-mode')
        ? '<i class="bi bi-sun-fill"></i>'
        : '<i class="bi bi-moon"></i>';
});