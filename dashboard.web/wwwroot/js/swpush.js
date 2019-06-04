const $subscribeButton = () => $('#subscribe-btn');
const $unsubscribeButton = () => $('#unsubscribe-btn');

class SwPush {
    isSubscribed = false;
    swRegistration = null;

    constructor(swRegistration) {
        this.swRegistration = swRegistration
        const _this = this;
        // Set the initial subscription value
        swRegistration.pushManager.getSubscription()
            .then((subscription) => {
                this.isSubscribed = !(subscription === null);

                if (this.isSubscribed) {
                    console.log('User IS subscribed.');
                } else {
                    console.log('User is NOT subscribed.');
                }

                this.updateBtn()
            });
    }

    urlB64ToUint8Array(base64String) {
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

    updateSubscriptionOnServer(subscription) {
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

    subscribeUser() {
        $.ajax({
            type: "GET",
            url: '/api/ServiceWorker/key',
            success: success => { console.log('got success', success) },
            error: err => console.log(err)
        })
            .then(applicationServerPublicKey => {
                const applicationServerKey = this.urlB64ToUint8Array(applicationServerPublicKey);
                this.swRegistration.pushManager.subscribe({
                    userVisibleOnly: true,
                    applicationServerKey: applicationServerKey
                })
                    .then((subscription) => {
                        console.log('User is subscribed.');

                        this.updateSubscriptionOnServer(subscription);

                        this.isSubscribed = true;

                        this.updateBtn();
                    })
                    .catch((err) => {
                        console.log('Failed to subscribe the user: ', err);
                        this.updateBtn();
                    });
            })
            .catch(err => {
                console.error('error connecting to server', err)
            });
    }

    unsubscribeUser() {
        this.swRegistration.pushManager.getSubscription()
            .then((subscription) => {
                if (subscription) {
                    return subscription.unsubscribe();
                }
            })
            .catch((error) => {
                console.log('Error unsubscribing', error);
            })
            .then(() => {
                this.updateSubscriptionOnServer(null);

                console.log('User is unsubscribed.');
                this.isSubscribed = false;

                this.updateBtn();
            });
    }

    updateBtn() {
        if (this.isSubscribed) {
            $unsubscribeButton().removeClass('hide')
            $subscribeButton().addClass('hide')
        } else {
            $unsubscribeButton().addClass('hide')
            $subscribeButton().removeClass('hide')
        }
    }
}

if ('serviceWorker' in navigator && 'PushManager' in window) {
    console.log('Service Worker and Push is supported');

    navigator.serviceWorker.register('js/sw.js')
        .then(function (swReg) {
            console.log('Service Worker is registered', swReg);

            const swPush = new SwPush(swReg);

            $subscribeButton().click(() => swPush.subscribeUser())
            $unsubscribeButton().click(() => swPush.unsubscribeUser())
        })
        .catch(function (error) {
            console.error('Service Worker Error', error);
        });
} else {
    console.warn('Push messaging is not supported');
    pushButton.textContent = 'Push Not Supported';
}
