self.addEventListener('push', function (event) {
    console.log('[Service Worker] Push Received.');
    if (event.data !== null) {
        const data = event.data.text();
        console.log(`[Service Worker] Push had this data: "${data}"`);

        const title = 'Reminder';
        const options = {
            body: data
        };

        const notificationPromise = self.registration.showNotification(title, options);
        event.waitUntil(notificationPromise);
    } else {
        console.log('no data', event)
    }
});

self.addEventListener('notificationclick', function (event) {
    console.log('[Service Worker] Notification click Received.');

    event.notification.close();
});