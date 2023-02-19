const swaggerUiTheme = {
    isDark: false,
    init: () => {
        let dark = window.matchMedia && window.matchMedia('(prefers-color-scheme: dark)').matches;
        if (!dark) {
            dark = localStorage.getItem('dark-mode');
        }
        swaggerUiTheme.update(dark);

        swaggerUiTheme.bindToggleButton();
    },
    update: (dark) => {
        if (dark) {
            document.body.classList.add("dark");
            localStorage.setItem('dark-mode', 1);
        } else {
            document.body.classList.remove("dark");
            localStorage.removeItem('dark-mode');
        }
        swaggerUiTheme.isDark = dark;
    },
    bindToggleButton: () => {
        const topBar = document.querySelector('.topbar-wrapper');
        if (!topBar) {
            setTimeout(() => {
                swaggerUiTheme.bindToggleButton();
            }, 250);
            return;
        }
        const button = document.createElement('button');
        button.innerHTML = swaggerUiTheme.isDark ? '☀️' : '🌙';
        button.classList.add('btn');
        button.classList.add('theme-toggle');
        button.addEventListener('click', () => {
            console.log('click');
            swaggerUiTheme.update(!swaggerUiTheme.isDark);
            button.innerHTML = swaggerUiTheme.isDark ? '☀️' : '🌙';
        });
        topBar.append(button);
    },
}


window.addEventListener('load', swaggerUiTheme.init);
