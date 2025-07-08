

$(function () {
	// Your jQuery code here
});

$(function () {

	//const connection = new signalR.HubConnectionBuilder()
	//	.withUrl("/OrderingHub")
	//	.build();
	//connection.on("OrderCompleteNotification", function (message) {
	//	try {
	//		var responsobj = JSON.parse(message);
	//		if (responsobj.status == 1) {
	//			var divorder = $('.myOrderActionPanel[data-orderid="' + responsobj.OrderId.toString() + '"]')
	//			var divstatus = $('.myOrderStatusPanel[data-orderid="' + responsobj.OrderId.toString() + '"]')
	//			divorder.addClass("d-none");
	//			divstatus.html('<span class="badge bg-success rounded-pill px-3 py-2 shadow-sm d-inline-flex align-items-center"><i class="bi bi-check-circle"></i></span>');
	//		}

	//	} catch (err) {
	//		console.error(err);
	//	}
	//});
	//connection.start().catch(function (err) {
	//	return console.error(err.toString());
	//});

	//$(".btn_request_to_conform").click(function (e) {
	//	const button = event.currentTarget
	//	var orderId = button.getAttribute('data-bs-foodorderid');
	//	var ordername = button.getAttribute('data-bs-ordername');
	//	var userempid = button.getAttribute('data-bs-userempid');
	//	var username = button.getAttribute('data-bs-username');
	//	var orderqnt = button.getAttribute('data-bs-orderqnt');

	//	try {
	//		connection.invoke("RequestForOrderComplete", orderId, ordername, orderqnt, userempid, username)
	//			.then((m) => {
	//				//console.log('Hub request send ');
	//				showInfoMessage("Request has been sent for confirmation. Request will be removed after 10 sec. ");
	//			})
	//			.catch(function (err) {
	//				showErrorMessage("Some error happened.Please refresh Page.");
	//				return console.error(err.toString());

	//			});
	//	} catch (err) {
	//		console.error(err);

	//	}
	//});

	const tooltipTriggerList = document.querySelectorAll('[data-bs-toggle="tooltip"]')
	const tooltipList = [...tooltipTriggerList].map(tooltipTriggerEl => new bootstrap.Tooltip(tooltipTriggerEl))

	$(".btn_Cancle_Item").click(function (e) {

		const button = e.currentTarget
		var orderid = button.getAttribute('data-bs-orderid');
		var foodorderid = button.getAttribute('data-bs-foodorderid');


	});

	function sendRequestForCancel(orderid, foodorderid) {
		
		try {
			const foodOrderdata = {
				"orderid": orderid.toString(),
				"foodOrderId": foodorderid.toString()
			};
			// Send the POST request using fetch
			fetch("MyOrders/cancelOrder", {
				method: "POST",
				headers: {
					"Content-Type": "application/json"
				},
				body: JSON.stringify(foodOrderdata)
			})
				.then((response) => response.json())
				.then((data) => {
					if (data.isDeleted === "ok") {
						$('tr[data-orderid="' + foodorderid.toString() + '"]').remove();
						//btn.closest('tr').remove();
					}
					//

				})
				.catch((error) => console.error("Error:", error));

		}
		catch (error) {
			console.log(error);
		}
	}

	$("#removeOrderDialog").on('show.bs.modal', function (e) {

		var button = e.relatedTarget;
		var foodorderid = button.getAttribute('data-bs-foodorderid');
		var orderid = button.getAttribute('data-bs-orderid');
		$("#removeOrderDialog").find('#orderid').val(button.getAttribute('data-bs-orderid'));
		$("#removeOrderDialog").find('#foodorderid').val(button.getAttribute('data-bs-foodorderid'));
		$("#removeOrderDialog").find('#panelname').val(button.getAttribute('data-bs-paneltitle'));
		$("#removeOrderDialog").find('#itemname').html(button.getAttribute('data-bs-foodordername'));
	});

	$("#removeOrderDialog").on("click", "#removemodaldelete", function () {
		var orderid = $("#removeOrderDialog").find('#orderid').val();
		var foodorderid = $("#removeOrderDialog").find('#foodorderid').val();
		var panelname = $("#removeOrderDialog").find('#panelname').val();
		sendRequestForCancel(orderid, foodorderid);

		//$("#removeOrderDialog").modal('toggle');
		var divcard = $("div[data-bs-panelname='" + panelname + "']");
		var rlen = $("div[data-bs-panelname='" + panelname + "']").first('table').find(' tbody tr').length;
		if (rlen == 0) {
			divcard.addClass("d-none");
		}
	});

	$("#myModal").on("click", ".btn-primary", function () {
		// code
	});

});