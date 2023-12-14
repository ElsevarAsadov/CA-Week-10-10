let deleteBtns = document.querySelectorAll(".delete-btn");

deleteBtns.forEach(btn => btn.addEventListener("click", function (e) {
    e.preventDefault();
    let url = btn.getAttribute("href");
    Swal.fire({
        title: "Are you sure?",
        text: "You won't be able to revert this!",
        icon: "warning",
        showCancelButton: true,
        confirmButtonColor: "#3085d6",
        cancelButtonColor: "#d33",
        confirmButtonText: "Yes, delete it!"
    }).then((result) => {
        if (result.isConfirmed) {

            fetch(url)
                .then(response => {
                    if (response.status == 200) {
                        window.location.reload(true);
                    } else {
                        alert("tapilmadi")
                    }
                })
        }
    });
}))


// Add to basket with Fetch
let addToBasketBns = document.querySelectorAll(".add-to-basket");


addToBasketBns.forEach(btn => btn.addEventListener("click", function (e) {
    let url = btn.getAttribute("href");

    e.preventDefault();

    fetch(url)
        .then(response => {
            if (response.status == 200) {
                alert("Added to basket")
            } else {
                alert("error!")
            }
        })


}))


// Book modal

const btns = document.querySelectorAll(".quick-modal-btn-shahin");

btns.forEach(btn => {
    btn.addEventListener("click", function (e) {
        e.preventDefault();
        const url = btn.getAttribute("href");
        console.log(url)
        fetch(url)
            .then(response => response.text())
            .then(data => {
                const modal = document.querySelector(".modal-dialog");
                modal.innerHTML = data
            })
    })
})