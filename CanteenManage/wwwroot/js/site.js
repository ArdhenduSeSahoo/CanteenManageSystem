// Please see documentation at https://learn.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.

// Write your JavaScript code.
function showErrorMessage(messages) {
    $("#errorModal #emsg").text(messages);
    $("#errorModal").modal('show');
}
function showInfoMessage(messages) {
    $("#infoModal #imsg").text(messages);
    $("#infoModal").modal('show');
}

function addFoodItem(e, api_Url) {
    //debugger;
    var foodid = $(e.currentTarget).data("foodid");
    //console.log("Add Item button clicked---"+foodid);
    try {
        const foodOrderdata = {
            foodOrderId: foodid.toString()
        };
        // Send the POST request using fetch
        fetch(api_Url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(foodOrderdata)
        })
            .then((response) => response.json())
            .then((data) => {
                //debugger;
                console.log("Success:", data);
                if (data.error != null && data.error != "") {
                    // your code here.
                    showErrorMessage(data.error);
                }
                else {
                    $('.btn_food_quantity[data-foodid="' + foodid + '"]').html(data.food_quantity);
                    $('.food_total_quantity').html(data.total_quantity);

                    if (data.food_quantity > 0) {
                        $('.food_add_remove_btn_div[data-foodid="' + foodid + '"]').removeClass("d-none");
                        $('.food_add_btn_div[data-foodid="' + foodid + '"]').addClass("d-none");
                    }
                    debugger;
                    if (data.total_quantity_cart > 0) {
                        $('.cartDiv').removeClass("d-none");
                        $('.cartValue').html(data.total_quantity_cart);
                    }
                    else {
                        $('.cartDiv').addClass("d-none");
                        $('.cartValue').html("0");
                    }
                    if (data.message != null && data.message != "") {
                        showInfoMessage(data.message);
                    }
                }

            })
            .catch((error) => console.error("Error:", error));

    }
    catch (error) {
        console.log(error);
    }
}

function removeFoodItem(e, api_Url) {
    //debugger;
    var foodid = $(e.currentTarget).data("foodid");
    //console.log("Add Item button clicked---"+foodid);
    try {
        const foodOrderdata = {
            foodOrderId: foodid.toString()
        };
        // Send the POST request using fetch
        fetch(api_Url, {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify(foodOrderdata)
        })
            .then((response) => response.json())
            .then((data) => {

                //console.log("Success:", data);
                if (data.error != null && data.error != "") {
                    // your code here.
                    showErrorMessage(data.error);
                }
                else {
                    $('.btn_food_quantity[data-foodid="' + foodid + '"]').html(data.food_quantity);
                    $('.food_total_quantity').html(data.total_quantity);
                    //showInfoMessage("Ardhendu");
                    if (data.food_quantity <= 0) {
                        $('.food_add_remove_btn_div[data-foodid="' + foodid + '"]').addClass("d-none");
                        $('.food_add_btn_div[data-foodid="' + foodid + '"]').removeClass("d-none");
                    }
                    if (data.total_quantity_cart > 0) {
                        $('.cartDiv').removeClass("d-none");
                        $('.cartValue').html(data.total_quantity_cart);
                    }
                    else {
                        $('.cartDiv').addClass("d-none");
                        $('.cartValue').html("0");
                    }
                }

            })
            .catch((error) => console.error("Error:", error));

    }
    catch (error) {
        console.log(error);
    }
}