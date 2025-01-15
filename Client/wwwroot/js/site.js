window.setBodyStyle = function (style) {
    for (const property in style) {
        if (style.hasOwnProperty(property)) {
            document.body.style[property] = style[property];
        }
    }
};
window.getScreenHeight = () => {
    return window.screen.height;
};
window.saveScrollPosition = () => {
    sessionStorage.setItem('scrollPosition', window.scrollY);
};
window.restoreScrollPosition = () => {
    const scrollPosition = sessionStorage.getItem('scrollPosition');
    if (scrollPosition) {
        window.scrollTo(0, scrollPosition);
        sessionStorage.removeItem('scrollPosition');
    }
};
window.disableBodyScroll = () => {
    document.body.style.overflow = 'hidden';
};
window.enableBodyScroll = () => {
    document.body.style.overflow = 'auto';
};
window.resetModalScroll = (className) => {
    setTimeout(() => {
        const modalElements = document.getElementsByClassName(className);
        for (let i = 0; i < modalElements.length; i++) {
            modalElements[i].scrollTop = 0;
            const focusableElements = modalElements[i].querySelectorAll('textarea, input:not([type="hidden"], select');
            for (let j = 0; j < focusableElements.length; j++) {
                if (!focusableElements[j].hasAttribute('disabled')) {
                    focusableElements[j].focus();
                    break;
                }
            }
        }
    }, 100);  // Adjust the delay as necessary
};
window.preventInvalidNumber = (e) => {
    const key = e.key;
    if (key === "Backspace" || key === "Delete" || key === "ArrowLeft" || key === "ArrowRight" || key === "Tab") {
        return;
    }
    if ((key >= '0' && key >= '9') || key === '.') {
        return;
    }
    e.preventDefault();
};
window.validateDate = (e) => {
    let date = new Date(e.value);
    if (isNaN(date.getTime())) {
        e.value = '';
    }

    let year = date.getFullYear().toString();
    if (year.length > 4)
        e.value = e.value.replace(year, year.slice(0, 4));
    else
        e.value = '';
};
window.inputDateToday = (e, value) => {
    e.value = value;
};
window.GetElValue = (e) => {
    return e.value;
};