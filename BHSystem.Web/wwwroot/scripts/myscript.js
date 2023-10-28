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

// Inner button header
window.InnerButton = () => {
  const element = document.getElementById("w1PositionStatic");
  const elementAdded = document.getElementById("w1InnerButton");
  if (elementAdded) {
    elementAdded.classList = "";
    element.appendChild(elementAdded);
  }
};

// Inner Pagination Grid
function InnerHmmlDxDataGrid(containerID, paginationId) {
  setTimeout(function () {
    //  lấy  ra pager
    var pager = document.getElementById(paginationId);
    pager.classList = ""; // xóa bỏ class d-none;
    var newElement = document.createElement("div"); // tạo một  thẻ div
    newElement.classList = "card-body dxbs-grid-pager dx-resize"; // class
    newElement.appendChild(pager);

    // inner html in dxDataGrid
    document
      .querySelector(`#${containerID} > div.card`)
      .appendChild(newElement);
  }, 1000);
}

// lưu thông tin file -> export excel
//hainguyen created
function saveAsFile(filename, bytesBase64) {
  if (navigator.msSaveBlob) {
    //Download cho in Edge browser
    var data = window.atob(bytesBase64);
    var bytes = new Uint8Array(data.length);
    for (var i = 0; i < data.length; i++) {
      bytes[i] = data.charCodeAt(i);
    }
    var blob = new Blob([bytes.buffer], { type: "application/octet-stream" });
    navigator.msSaveBlob(blob, filename);
  } else {
    var link = document.createElement("a");
    link.download = filename;
    link.href = "data:application/octet-stream;base64," + bytesBase64;
    document.body.appendChild(link); // Needed for Firefox
    link.click();
    document.body.removeChild(link);
  }
}

// lấy thông tin fieldname + caption
//hainguyen created
function getColumnNameDxDataGrid(containerID) {
  try {
    var listObj = [];
    var lstThs = document.querySelectorAll(
      `#${containerID} .dxbs-grid-header-container table thead tr th`
    ); // lấy caption

    var lstTds = document
      .querySelectorAll(
        `#${containerID} .dxbs-grid-hsd table tbody .dxbs-data-row`
      )[0] //longtran 20230531 gặp đúng class dxbs-data-row thì mới lấy, tránh Trường hợp group by
      .querySelectorAll("td"); // lấy field name
    if (
      lstThs.length > 0 &&
      lstTds.length > 0 &&
      lstThs.length == lstTds.length
    ) {
      for (var i = 0; i < lstTds.length; i++) {
        if (lstTds[i].id != "") {
          listObj.push({ code: lstTds[i].id, name: lstThs[i].innerText });
        }
      }
    }
  } catch (e) {
    return listObj; //longtran 20230631 nếu rớt catch thì trả về danh sách rỗng
  }
  return listObj;
}

/*
    longtran 20230821 click vào khi muốn mở rộng/thu hẹp thuộc tính group by (đối với trường hợp đã mở rồi thì không mở nữa, đóng lại rồi thì không đóng nữa)
 */

function autoClickButtonGroupbyParentLine(gridId, type) {
  try {
    var grid = document.getElementById(gridId); //tìm grid  document.querySelector(".my-grid");
    var numberOfElementsDxbs = document.querySelectorAll(
      ".dxbs-grid-first-level-group"
    ).length; // số lượng thẻ tr có class là dxbs-grid-first-level-group
    for (var i = 1; i <= numberOfElementsDxbs; i++) {
      if (type == "want_to_open") {
        //muốn mở ra
        var buttonGroupParentNoOpen = document
          .querySelectorAll(
            `#${gridId} .dxbs-grid-hsd table tbody .dxbs-grid-first-level-group`
          )
          [i - 1].querySelectorAll("td")[0]
          .querySelectorAll("button")[0]
          .querySelectorAll(".dx-image-rotate-270")[0]; // lấy icon group cha chưa mở
        if (buttonGroupParentNoOpen != null) {
          //nếu tồn tại icon group cha chưa được mở ra
          document
            .querySelectorAll(
              `#${gridId} .dxbs-grid-hsd table tbody .dxbs-grid-first-level-group`
            )
            [i - 1].querySelectorAll("td")[0]
            .querySelectorAll("button")[0]
            .click(); //click vào để mở ra
        }
      } else if (type == "want_to_close") {
        var buttonGroupParentOpen = document
          .querySelectorAll(
            `#${gridId} .dxbs-grid-hsd table tbody .dxbs-grid-first-level-group`
          )
          [i - 1].querySelectorAll("td[rowspan]")[0];
        if (buttonGroupParentOpen != null) {
          //nếu tồn tại icon group cha được mở ra
          document
            .querySelectorAll(
              `#${gridId} .dxbs-grid-hsd table tbody .dxbs-grid-first-level-group`
            )
            [i - 1].querySelectorAll("td")[0]
            .querySelectorAll("button")[0]
            .click(); //click vào để đóng lại ra
        }
      }
    }
  } catch (e) {
    return;
  }
}

/*
    longtran 20230602  click vào khi muốn mở rộng/thu hẹp thuộc tính group by (đối với trường hợp đã mở rồi thì không mở nữa, đóng lại rồi thì không đóng nữa) đối với group by trong groupby
 */
function autoClickButtonGroupbyChildLine(gridId, type) {
  try {
    var grid = document.getElementById(gridId); //tìm grid  document.querySelector(".my-grid");
    var numberOfElementsDataGroup = document.querySelectorAll(
      `#${gridId} .dxbs-grid-hsd table tbody tr[data-group="1"]`
    ).length;
    for (var j = 1; j <= numberOfElementsDataGroup; j++) {
      if (type == "want_to_open") {
        //muốn mở ra
        var buttonGroupChildNoOpen = document
          .querySelectorAll(
            `#${gridId} .dxbs-grid-hsd table tbody tr[data-group="1"]`
          )
          [j - 1].querySelectorAll("td")[0]
          .querySelectorAll("button")[0]
          .querySelectorAll(".dx-image-rotate-270")[0]; // lấy icon group con chưa mở
        if (buttonGroupChildNoOpen != null) {
          //nếu tồn tại icon group cha chưa được mở ra
          document
            .querySelectorAll(
              `#${gridId} .dxbs-grid-hsd table tbody tr[data-group="1"]`
            )
            [j - 1].querySelectorAll("td")[0]
            .querySelectorAll("button")[0]
            .click(); //click vào để mở ra
        }
      } else if (type == "want_to_close") {
        // muốn đóng lại
        var buttonGroupChildOpen = document
          .querySelectorAll(
            `#${gridId} .dxbs-grid-hsd table tbody tr[data-group="1"]`
          )
          [j - 1].querySelectorAll("td[rowspan]")[0];
        if (buttonGroupChildOpen != null) {
          //nếu tồn tại icon group cha được mở ra
          document
            .querySelectorAll(
              `#${gridId} .dxbs-grid-hsd table tbody tr[data-group="1"]`
            )
            [j - 1].querySelectorAll("td")[0]
            .querySelectorAll("button")[0]
            .click(); //click vào để đóng lại ra
        }
      }
    }
  } catch (e) {
    return;
  }
}

/*
    longtran 20230821 click vào khi muốn mở rộng hoặc thu hẹp các thuộc tính group by
 */

//function autoClickButtonExtendGroupbyParentLine(gridId) {
//    try {
//        var grid = document.getElementById(gridId);//tìm grid  document.querySelector(".my-grid");
//        var numberOfElementsDxbs = document.querySelectorAll(".dxbs-grid-first-level-group").length; // số lượng thẻ tr có class là dxbs-grid-first-level-group
//        for (var i = 1; i <= numberOfElementsDxbs; i++) {
//            var firstButtonGroupParent = document.querySelectorAll(`#${gridId} .dxbs-grid-hsd table tbody .dxbs-grid-first-level-group`)[i-1].querySelectorAll("td")[0].querySelectorAll("button")[0];
//            if (firstButtonGroupParent != null) {
//                firstButtonGroupParent.click(); //click ở group by cha
//            }
//        }
//    } catch (e) {
//        return;
//    }
//}

///*
//    longtran 20230602 tự động click button group by lòng group by
// */
//function autoClickButtonExtendGroupbyChildLine(gridId) {
//    try {
//        var grid = document.getElementById(gridId);//tìm grid  document.querySelector(".my-grid");
//        var numberOfElementsDataGroup = document.querySelectorAll(`#${gridId} .dxbs-grid-hsd table tbody tr[data-group="1"]`).length;
//        for (var j = 1; j <= numberOfElementsDataGroup; j++) {
//            var firstButtonGroupChild = document.querySelectorAll(`#${gridId} .dxbs-grid-hsd table tbody tr[data-group="1"]`)[j - 1].querySelectorAll("td")[0].querySelectorAll("button")[0];
//            if (firstButtonGroupChild != null) {
//                firstButtonGroupChild.click();
//            }
//        }
//    } catch (e) {
//        return;
//    }
//}

/*
    longtran 20230602 tự động click button group by dòng đầu tiên 
 */
function autoClickButtonGroupbyLine0(gridId) {
  try {
    var grid = document.getElementById(gridId); //tìm grid  document.querySelector(".my-grid");
    var firstButtonGroup0 = document
      .querySelectorAll(
        `#${gridId} .dxbs-grid-hsd table tbody .dxbs-grid-first-level-group`
      )[0]
      .querySelectorAll("td")[0]
      .querySelectorAll("button")[0];
    if (firstButtonGroup0 != null) {
      firstButtonGroup0.click();
    }
  } catch (e) {
    return;
  }
}

/*
    longtran 20230602 tự động click button group by lòng group by 
 */
function autoClickButtonGroupbyLine1(gridId) {
  try {
    var grid = document.getElementById(gridId); //tìm grid  document.querySelector(".my-grid");
    var firstButtonGroup1 = document
      .querySelectorAll(`#${gridId} .dxbs-grid-hsd table tbody tr`)[1]
      .querySelectorAll("td")[0]
      .querySelectorAll("button")[0];
    if (firstButtonGroup1 != null) {
      firstButtonGroup1.click();
    }
  } catch (e) {
    return;
  }
}

window.downloadFileFromStream = async (fileName, contentStreamReference) => {
  const arrayBuffer = await contentStreamReference.arrayBuffer();
  const blob = new Blob([arrayBuffer]);
  const url = URL.createObjectURL(blob);
  const anchorElement = document.createElement("a");
  anchorElement.href = url;
  anchorElement.download = fileName ?? "";
  anchorElement.click();
  anchorElement.remove();
  URL.revokeObjectURL(url);
};

/*
    20230425 haile set title cho page
    
 */
function SetTitle(tilte) {
  document.title = tilte;
}

/*
    haile 20230628 dxdatagrid, gán nội dung filter
 */
function setContentFilter(gridId, colId, content) {
  var grid = document.getElementById(gridId); //tìm grid  document.querySelector(".my-grid");

  if (grid == null) return;

  var filterRow = grid.querySelector(".dxbs-filter-row");

  var inputs = filterRow.querySelectorAll("input");
  if (inputs != null && inputs.length > colId) {
    inputs[colId].value = content;
  }
}

/*
    longtran 20230712 mở page giúp đỡ
 */
function currsorIndex(id) {
  try {
    var height = window.screen.availHeight - 100;
    var width = window.screen.availWidth - 180;
    window.open(
      "../Help/html/" + id + ".html",
      "width=" + width + "," + "height=" + height + ",top=30" + ",left=100",
      "location=yes, scrollbars = yes, status = yes"
    );
  } catch (e) {
    return;
  }
}

function adjustHeight() {
  const windowHeight = window.innerHeight;
  const dynamicElement = document.querySelector(".dynamic-height");
  if (dynamicElement) {
    dynamicElement.style.height = `calc(${windowHeight}px - 118px)`;
  }
}

// Gọi hàm adjustHeight() khi cửa sổ trình duyệt được tải và mỗi khi thay đổi kích thước cửa sổ.
window.addEventListener("load", adjustHeight);
window.addEventListener("resize", adjustHeight);

// focus input
function focusInput(id) {
  var input = document.getElementById(id);
  if (input) {
    input.focus();
  }
}
