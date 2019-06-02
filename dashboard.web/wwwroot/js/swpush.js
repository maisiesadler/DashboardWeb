let isSubscribed = false;
let swRegistration = null;
const $subscribeButton = () => $('#subscribe-btn');
const $unsubscribeButton = () => $('#unsubscribe-btn');

function urlB64ToUint8Array(base64String) {
    const padding = '='.repeat((4 - base64String.length % 4) % 4);
    const base64 = (base64String + padding)
        .replace(/\-/g, '+')
        .replace(/_/g, '/');

    const rawData = window.atob(base64);
    const outputArray = new Uint8Array(rawData.length);

    for (let i = 0; i < rawData.length; ++i) {
        outputArray[i] = rawData.charCodeAt(i);
    }
    return outputArray;
}

function updateSubscriptionOnServer(subscription) {
    const data = subscription === null ? null : JSON.stringify(subscription)
    console.log('sending subscription', data)

    $.ajax({
        type: "POST",
        url: '/api/ServiceWorker',
        data: data,
        success: success => { console.log('got success', success) },
        error: err => console.log(err),
        contentType: 'application/json',
        dataType: 'json'
    });
}

function unsubscribeUser() {
    swRegistration.pushManager.getSubscription()
        .then(function (subscription) {
            if (subscription) {
                return subscription.unsubscribe();
            }
        })
        .catch(function (error) {
            console.log('Error unsubscribing', error);
        })
        .then(function () {
            updateSubscriptionOnServer(null);

            console.log('User is unsubscribed.');
            isSubscribed = false;

            updateBtn();
        });
}

function updateBtn() {
    if (isSubscribed) {
        $unsubscribeButton().removeClass('hide')
        $subscribeButton().addClass('hide')
    } else {
        $unsubscribeButton().addClass('hide')
        $subscribeButton().removeClass('hide')
    }
}

document.addEventListener('DOMContentLoaded', () => {
    function initializeUI() {
        // Set the initial subscription value
        swRegistration.pushManager.getSubscription()
            .then(function (subscription) {
                isSubscribed = !(subscription === null);

                if (isSubscribed) {
                    console.log('User IS subscribed.');
                } else {
                    console.log('User is NOT subscribed.');
                }

                updateBtn()
            });

        $subscribeButton().click(() => subscribeUser())
        $unsubscribeButton().click(() => unsubscribeUser())
    }

    if ('serviceWorker' in navigator && 'PushManager' in window) {
        console.log('Service Worker and Push is supported');

        navigator.serviceWorker.register('js/sw.js')
            .then(function (swReg) {
                console.log('Service Worker is registered', swReg);

                swRegistration = swReg;
                initializeUI()
            })
            .catch(function (error) {
                console.error('Service Worker Error', error);
            });
    } else {
        console.warn('Push messaging is not supported');
        pushButton.textContent = 'Push Not Supported';
    }
})

function subscribeUser() {
    $.ajax({
        type: "GET",
        url: '/api/ServiceWorker/key',
        success: success => { console.log('got success', success) },
        error: err => console.log(err)
    })
        .then(applicationServerPublicKey => {
            const applicationServerKey = urlB64ToUint8Array(applicationServerPublicKey);
            swRegistration.pushManager.subscribe({
                userVisibleOnly: true,
                applicationServerKey: applicationServerKey
            })
                .then(function (subscription) {
                    console.log('User is subscribed.');

                    updateSubscriptionOnServer(subscription);

                    isSubscribed = true;

                    updateBtn();
                })
                .catch(function (err) {
                    console.log('Failed to subscribe the user: ', err);
                    updateBtn();
                });
        })
        .catch(err => {
            console.error('error connecting to server', err)
        });

}

