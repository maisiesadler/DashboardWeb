$('.triggered [close]')
    .each((i, v) => {
        const id = v.getAttribute('close')
        $(v).click(() => {
            console.log('attempting close')
            $.ajax({
                type: "PUT",
                url: '/api/Reminders/done',
                data: { id },
                success: _ => location.reload(),
                error: err => console.log(err)
            })
        })
    })
