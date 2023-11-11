// lắng nghe sự kiện -> khi các element trong body thay đổi.
new MutationObserver((mutations, observer) => {
  // gọi hàm khi UI chuyển sang trạng thai -> reconnect-failed
  //reconnect-rejected
  if (
    document.querySelector(
      "#components-reconnect-modal.components-reconnect-failed .m1-custom-failed h1"
    ) ||
    document.querySelector(
      "#components-reconnect-modal.components-reconnect-rejected .m1-custom-failed h1"
    )
  ) {
    const Reconnection = async () => {
      // gọi api -> nếu status == 200, tự động reload trang
      //await fetch("http://server3.onesystem.vn:8094/MM1Api/GetCompany", {
      //  method: "GET",
      //  mode: "no-cors", // no-cors, *cors, same-origin
      //  headers: {
      //    "Access-Control-Allow-Origin": "*",
      //    "Access-Control-Allow-Methods": "GET",
      //    "Access-Control-Allow-Headers":
      //      "Origin, X-Requested-With, Content-Type, Accept, Authorization",
      //    "Content-Type": "application/json",
      //  },
      //});
      await fetch("");

      location.reload(); // bắt reload lại
    };
    observer.disconnect();
    Reconnection();
    setInterval(Reconnection, 3500);
  }
}).observe(document.getElementById("components-reconnect-modal"), {
  attributes: true,
  characterData: true,
  childList: true,
  subtree: true,
  attributeOldValue: true,
  characterDataOldValue: true,
});

// focus input
function focusInput(id) {
  var input = document.getElementById(id);
  if (input) {
    input.focus();
  }
}
