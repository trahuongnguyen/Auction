'use strict';

const modal = document.querySelectorAll('.modal-login');
//const modal = document.getElementsByClassName('modal');
const overlay = document.querySelector('.overlay');
const btnCloseModal = document.querySelectorAll('.close-modal');
const btnShowModal = document.querySelectorAll('.show-modal');


// const openModal = function () {
//     // overlay.classList.remove('hidden');
//     //modal.classList.add('sticky-top-login');
//     modal[arguments[0]].classList.remove('hidden');
//     // overlay.classList.add('sticky-top-login');
// };

// function closeModal() {
//     modal[arguments].classList.add('hidden');
//     overlay.classList.add('hidden');
// }

for (let i = 0; i <= btnShowModal.length - 1; i++) {
    btnShowModal[i].addEventListener('click', () => {
        overlay.classList.remove('hidden');
        //modal.classList.add('sticky-top-login');
        modal[i].classList.remove('hidden');
        // overlay.classList.add('sticky-top-login');
    });
   
    btnCloseModal[i].addEventListener('click', () => {
        modal[i].classList.add('hidden');
        overlay.classList.add('hidden');
    });
    overlay.addEventListener('click', () => {
        modal[i].classList.add('hidden');
        overlay.classList.add('hidden');
    });

    document.addEventListener('keydown', function (e) {
        if (e.key === 'Escape') {
            if (!modal[i].classList.contains('hidden')) {
                () => {
                    modal[i].classList.add('hidden');
                    overlay.classList.add('hidden');
                };
            }
        }
    });
}