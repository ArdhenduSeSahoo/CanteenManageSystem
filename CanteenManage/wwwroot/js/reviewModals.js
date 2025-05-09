const exampleModal = document.getElementById('exampleModal')
if (exampleModal) {
    exampleModal.addEventListener('show.bs.modal', event => {
        // Button that triggered the modal
        console.log("enentfire");
        const button = event.relatedTarget
        // Extract info from data-bs-* attributes
        var orderId = button.getAttribute('data-bs-orderid');
        console.log(orderId);
        var modalOrderID_Hidden = exampleModal.querySelector('#order_id');
        modalOrderID_Hidden.value = orderId;

    })
}