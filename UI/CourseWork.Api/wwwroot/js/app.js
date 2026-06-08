const api = '/api';
let auth = JSON.parse(localStorage.getItem('auth') || 'null');

const tourTypeNames = ['Екскурсійний', 'Пляжний', 'Гірськолижний', 'Круїз', 'Культурний'];

function headers() {
    const h = { 'Content-Type': 'application/json' };
    if (auth?.token) h['Authorization'] = `Bearer ${auth.token}`;
    return h;
}

async function request(url, options = {}) {
    const response = await fetch(url, { ...options, headers: { ...headers(), ...options.headers } });
    if (!response.ok) {
        const err = await response.json().catch(() => ({ message: 'Помилка сервера' }));
        throw new Error(err.message || 'Помилка');
    }
    if (response.status === 204) return null;
    return response.json();
}

function updateAuthUi() {
    const userInfo = document.getElementById('userInfo');
    const loginBtn = document.getElementById('loginBtn');
    const logoutBtn = document.getElementById('logoutBtn');
    const manageTab = document.getElementById('manageTabItem');

    if (auth) {
        userInfo.textContent = `${auth.fullName} (${auth.role})`;
        loginBtn.classList.add('d-none');
        logoutBtn.classList.remove('d-none');
        if (auth.role === 'Manager' || auth.role === 'Administrator') {
            manageTab.classList.remove('d-none');
        }
    } else {
        userInfo.textContent = 'Гість — лише перегляд';
        loginBtn.classList.remove('d-none');
        logoutBtn.classList.add('d-none');
        manageTab.classList.add('d-none');
    }
}

function renderTours(container, tours, showBook = true) {
    container.innerHTML = tours.map(t => `
        <div class="col-md-4">
            <div class="card tour-card h-100 shadow-sm">
                <div class="card-body">
                    <div class="d-flex justify-content-between">
                        <h5 class="card-title">${t.title}</h5>
                        ${t.isHot ? '<span class="badge badge-hot">HOT</span>' : ''}
                    </div>
                    <p class="card-text small text-muted">${t.countryName}${t.regionName ? ', ' + t.regionName : ''}</p>
                    <p class="card-text">${t.description}</p>
                    <p><strong>${t.price} грн</strong> · ${tourTypeNames[t.type] ?? t.type}</p>
                    <p class="small">Місць: ${t.availablePlaces}</p>
                    ${showBook && auth ? `<button class="btn btn-sm btn-primary" onclick="bookTour(${t.id})">Забронювати</button>` : ''}
                </div>
            </div>
        </div>`).join('') || '<p class="text-muted">Турів не знайдено</p>';
}

async function loadTours(params = '') {
    const tours = await request(`${api}/tours?${params}`);
    renderTours(document.getElementById('toursList'), tours);
}

async function loadHotTours() {
    const tours = await request(`${api}/tours?isHot=true`);
    const section = document.getElementById('hotSection');
    section.innerHTML = '<div class="row g-3" id="hotList"></div>';
    renderTours(document.getElementById('hotList'), tours);
}

async function loadBookings() {
    if (!auth) {
        document.getElementById('bookingsSection').innerHTML = '<p class="text-warning">Увійдіть для перегляду бронювань</p>';
        return;
    }
    const tourBookings = await request(`${api}/bookings/tours`);
    const ticketBookings = await request(`${api}/ticketbookings`);
    document.getElementById('bookingsSection').innerHTML = `
        <h5>Бронювання турів</h5>
        <ul class="list-group mb-3">${tourBookings.map(b => `<li class="list-group-item">${b.tourTitle} — ${b.placesCount} міс., ${b.totalPrice} грн</li>`).join('') || '<li class="list-group-item">Немає</li>'}</ul>
        <h5>Квитки / номери</h5>
        <ul class="list-group">${ticketBookings.map(b => `<li class="list-group-item">${b.itemName} — ${b.totalPrice} грн</li>`).join('') || '<li class="list-group-item">Немає</li>'}</ul>`;
}

async function loadTickets() {
    const transports = await request(`${api}/reference/transports`);
    const rooms = await request(`${api}/reference/hotel-rooms`);
    document.getElementById('ticketsSection').innerHTML = `
        <div class="row g-4">
            <div class="col-md-6">
                <h5>Транспорт</h5>
                ${transports.map(t => `
                    <div class="border rounded p-3 mb-2">
                        <strong>${t.name}</strong> (${t.route}) — ${t.price} грн
                        ${auth ? `<button class="btn btn-sm btn-outline-primary float-end" onclick="bookTransport(${t.id})">Купити</button>` : ''}
                    </div>`).join('')}
            </div>
            <div class="col-md-6">
                <h5>Номери готелів</h5>
                ${rooms.map(r => `
                    <div class="border rounded p-3 mb-2">
                        <strong>${r.hotelName}</strong>, №${r.roomNumber} — ${r.pricePerNight} грн/ніч
                        ${auth ? `<button class="btn btn-sm btn-outline-primary float-end" onclick="bookRoom(${r.id})">Забронювати</button>` : ''}
                    </div>`).join('')}
            </div>
        </div>`;
}

window.bookTour = async (tourId) => {
    try {
        await request(`${api}/bookings/tours`, { method: 'POST', body: JSON.stringify({ tourId, placesCount: 1 }) });
        alert('Тур заброньовано!');
        loadTours();
    } catch (e) { alert(e.message); }
};

window.bookTransport = async (transportId) => {
    try {
        await request(`${api}/ticketbookings/transport`, { method: 'POST', body: JSON.stringify({ transportId, quantity: 1 }) });
        alert('Квиток заброньовано!');
        loadTickets();
    } catch (e) { alert(e.message); }
};

window.bookRoom = async (hotelRoomId) => {
    const checkIn = prompt('Дата заїзду (YYYY-MM-DD):');
    const checkOut = prompt('Дата виїзду (YYYY-MM-DD):');
    if (!checkIn || !checkOut) return;
    try {
        await request(`${api}/ticketbookings/hotel`, {
            method: 'POST',
            body: JSON.stringify({ hotelRoomId, checkInDate: checkIn, checkOutDate: checkOut })
        });
        alert('Номер заброньовано!');
        loadTickets();
    } catch (e) { alert(e.message); }
};

document.querySelectorAll('#mainTabs .nav-link').forEach(btn => {
    btn.addEventListener('click', () => {
        document.querySelectorAll('#mainTabs .nav-link').forEach(b => b.classList.remove('active'));
        btn.classList.add('active');
        document.querySelectorAll('.tab-panel').forEach(p => p.classList.add('d-none'));
        const tab = btn.dataset.tab;
        document.getElementById(`${tab}Section`).classList.remove('d-none');
        if (tab === 'hot') loadHotTours();
        if (tab === 'bookings') loadBookings();
        if (tab === 'tickets') loadTickets();
    });
});

document.getElementById('searchBtn').addEventListener('click', () => {
    const params = new URLSearchParams();
    const search = document.getElementById('searchInput').value;
    const type = document.getElementById('typeFilter').value;
    if (search) params.set('searchTerm', search);
    if (type) params.set('type', type);
    loadTours(params.toString());
});

document.getElementById('loginForm').addEventListener('submit', async e => {
    e.preventDefault();
    try {
        auth = await request(`${api}/auth/login`, {
            method: 'POST',
            body: JSON.stringify({ email: loginEmail.value, password: loginPassword.value })
        });
        localStorage.setItem('auth', JSON.stringify(auth));
        updateAuthUi();
        bootstrap.Modal.getInstance(document.getElementById('authModal')).hide();
    } catch (err) { authError.textContent = err.message; }
});

document.getElementById('registerForm').addEventListener('submit', async e => {
    e.preventDefault();
    try {
        auth = await request(`${api}/auth/register`, {
            method: 'POST',
            body: JSON.stringify({
                email: regEmail.value,
                password: regPassword.value,
                firstName: regFirstName.value,
                lastName: regLastName.value
            })
        });
        localStorage.setItem('auth', JSON.stringify(auth));
        updateAuthUi();
        bootstrap.Modal.getInstance(document.getElementById('authModal')).hide();
    } catch (err) { authError.textContent = err.message; }
});

document.getElementById('loginTab').addEventListener('click', () => {
    loginForm.classList.remove('d-none');
    registerForm.classList.add('d-none');
    loginTab.classList.add('active');
    registerTab.classList.remove('active');
});

document.getElementById('registerTab').addEventListener('click', () => {
    registerForm.classList.remove('d-none');
    loginForm.classList.add('d-none');
    registerTab.classList.add('active');
    loginTab.classList.remove('active');
});

document.getElementById('logoutBtn').addEventListener('click', () => {
    auth = null;
    localStorage.removeItem('auth');
    updateAuthUi();
});

document.getElementById('tourForm').addEventListener('submit', async e => {
    e.preventDefault();
    const fd = new FormData(e.target);
    const body = {
        title: fd.get('title'),
        description: fd.get('description'),
        type: Number(fd.get('type')),
        countryId: Number(fd.get('countryId')),
        price: Number(fd.get('price')),
        startDate: fd.get('startDate'),
        endDate: fd.get('endDate'),
        isHot: fd.get('isHot') === 'on',
        availablePlaces: Number(fd.get('availablePlaces'))
    };
    try {
        await request(`${api}/tours`, { method: 'POST', body: JSON.stringify(body) });
        alert('Тур створено');
        e.target.reset();
        loadTours();
    } catch (err) { alert(err.message); }
});

updateAuthUi();
loadTours();
