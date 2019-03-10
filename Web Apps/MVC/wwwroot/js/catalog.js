class Catalog {
    clickAddToBasket(el) {
        const btn = $(el);
        btn.attr('disabled', true);
        btn.removeClass('btn-success');
        btn.addClass('btn-secondary');
        let code = btn.attr('code');
        this.addToBasket(code);
    }

    addToBasket(code) {
        let token = $('[name=__RequestVerificationToken]').val();

        let headers = {};
        headers['RequestVerificationToken'] = token;

        $.ajax({
            url: '/basket/addtobasket',
            type: 'POST',
            contentType: 'application/json',
            data: JSON.stringify(code),
            headers: headers
        }).done(function (response) {
            let items = response.Items;
            catalog.showSnackbar();
            //let itemLine = $('[item-id=' + basketItem.Id + ']');
            //itemLine.find('input').val(basketItem.Quantity);
            //itemLine.find('[subtotal]').html((basketItem.Subtotal).twoDigits());
            //let customerBasket = response.CustomerBasket;
            //$('[item-qty]').html('Total: ' + customerBasket.Items.length + ' items');
            //$('[total]').html((customerBasket.Total).twoDigits());

            //if (basketItem.Quantity === 0) {
            //    itemLine.remove();
            //}
        });
    }

    showSnackbar() {
        var x = document.getElementById("snackbar");
        x.className = "show";
        setTimeout(function () { x.className = x.className.replace("show", ""); }, 3000);
    }
}

var catalog = new Catalog();
