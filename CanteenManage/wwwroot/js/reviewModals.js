

$(document).ready(function () {

    const exampleModal = document.getElementById('exampleModal')
    if (exampleModal) {
        exampleModal.addEventListener('show.bs.modal', event => {
            // Button that triggered the modal
            //console.log("enentfire");
            const button = event.relatedTarget
            // Extract info from data-bs-* attributes
            var orderId = button.getAttribute('data-bs-orderid');
            //console.log(orderId);
            var modalOrderID_Hidden = exampleModal.querySelector('#order_id');
            modalOrderID_Hidden.value = orderId;

        })
    }


    const exampleModal = document.getElementById('viewReviewModal')

    if (exampleModal) {
        exampleModal.addEventListener('show.bs.modal', event => {
            // Button that triggered the modal
            //console.log("enentfire");

            const button = event.relatedTarget
            // Extract info from data-bs-* attributes data-bs-rating
            var reviews = button.getAttribute('data-bs-review');
            //console.log(reviews);

            var rating = button.getAttribute('data-bs-rating');
            //console.log(rating);

            var modalreviewTextSpan = exampleModal.querySelector('#reviewText');
            var modaltatingImgTag = exampleModal.querySelector('#ratingimg');
            if (rating == 1) {
                modaltatingImgTag.src = "images/emoji/Emoji1.png";
            }
            else if (rating == 2) {
                modaltatingImgTag.src = "images/emoji/Emoji2.png";
            }
            else if (rating == 3) {
                modaltatingImgTag.src = "images/emoji/Emoji3.png";
            }
            else if (rating == 4) {
                modaltatingImgTag.src = "images/emoji/Emoji4.png";
            }
            else if (rating == 5) {
                modaltatingImgTag.src = "images/emoji/Emoji5.png";
            }
            else {
                modaltatingImgTag.src = "images/emoji/Emoji1.png";
            }
            modalreviewTextSpan.innerHTML = reviews;

        })
    }
});

