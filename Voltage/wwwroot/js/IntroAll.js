window.addEventListener('DOMContentLoaded', () => {

    const getStartedBtn = document.getElementById('getStartedBtn');

    getStartedBtn.addEventListener('click', () => {
        const messagesNavSection = document.querySelector(".messagesNavSection");
        const connectionNavSection = document.querySelector(".connectionNavSection");
        const profileNavSec = document.querySelector(".profileNavSec");
        console.log(messagesNavSection);

        introJs().setOptions({
            steps: [
                {
                    title: 'Welcome',
                    intro: 'Hello! Welcome to our website! I m your assistant.Right now, I ll show you around. If you have any questions or need assistance, feel free to reach out to us.'
                },
                {
                    element: messagesNavSection,
                    title: 'Chat',
                    intro: 'This is the chat section. Here, you can chat with friends.',
                    position: 'left',
                },
                {
                    element: connectionNavSection,
                    title: 'Contact',
                    intro: 'This is the profile section. Here, you can change your nickname, country, date of birth, password, and photo',
                    position: 'right',
                },
                {
                    element: profileNavSec,
                    title: 'Profile',
                    intro: 'This is the section for finding friends. Additionally, here you ll find a section for accepting friend requests',
                    position: 'right',
                },
            ]
        }).start();
    });
});
