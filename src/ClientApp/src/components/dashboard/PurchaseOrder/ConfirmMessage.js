const ConfirmMessage = (message, callback) => {
  var confirmModal = document.getElementById("confirm-modal");
  var confirmYes = document.getElementById("confirm-yes");
  var confirmNo = document.getElementById("confirm-no");

  confirmYes.addEventListener("click", function () {
    confirmModal.style.display = "none";
    callback(true);
  });

  confirmNo.addEventListener("click", function () {
    confirmModal.style.display = "none";
    callback(false);
  });

  confirmModal.style.display = "block";
};

export default ConfirmMessage;
