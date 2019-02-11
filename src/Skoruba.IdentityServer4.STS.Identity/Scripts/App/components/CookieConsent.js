window.addEventListener("load", function () {
    window.cookieconsent.initialise({
        "palette": {
            "popup": {
                "background": "#eaf7f7",
                "text": "#5c7291"
            },
            "button": {
                "background": "#56cbdb",
                "text": "#ffffff"
            }
        },
        "position": "bottom-right",
        "content": {
            "message": "This website uses cookies to keep you logged in and ensure that you get the best experience on our website. Your personal data never leaves this website until you explicitly allow for it in consent screen.",
        }
    });
});