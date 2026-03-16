const API_BASE = window.location.origin;

document.addEventListener('DOMContentLoaded', async () => {
    let path = window.location.pathname.replace(/^\/|\/$/g, '');
    let slug = '';
    
    if (path.startsWith('menu/')) {
        slug = path.replace('menu/', '');
    } else if (path === 'menu') {
        slug = '';
    } else {
        slug = path;
    }

    if (!slug || slug.includes('index.html')) {
        slug = 'demo';
    }

    const apiPrefix = window.location.pathname.includes('/menu') ? '/menu' : '';

    try {
        const resResponse = await fetch(`${API_BASE}${apiPrefix}/${slug}/restaurant`);
        if (!resResponse.ok) throw new Error('Restaurante no encontrado');
        const restaurant = await resResponse.json();

        // Banner & Header setup
        document.getElementById('logo').src = restaurant.logoUrl;
        document.getElementById('restaurant-name').textContent = restaurant.name;
        document.getElementById('restaurant-address').textContent = restaurant.address;
        
        if (restaurant.phone) {
            document.getElementById('restaurant-phone').textContent = restaurant.phone;
        }

        const bannerEl = document.getElementById('restaurant-banner');
        if (restaurant.bannerUrl) {
            bannerEl.style.backgroundImage = `url('${restaurant.bannerUrl}')`;
        } // else uses default gradient from CSS

        // WhatsApp Button
        if (restaurant.whatsappNumber) {
            const waBtn = document.getElementById('whatsapp-btn');
            waBtn.href = `https://wa.me/${restaurant.whatsappNumber}?text=Hola,%20vi%20el%20menú%20y%20quiero%20ordenar`;
            waBtn.classList.remove('hidden');
        }

        // Fetch Categories
        const catResponse = await fetch(`${API_BASE}${apiPrefix}/${slug}/categories`);
        const categories = await catResponse.json();
        
        if (categories.length > 0) {
            renderTabs(categories, slug, apiPrefix);
            await loadCategoryItems(slug, categories[0].id, apiPrefix);
        } else {
            document.getElementById('menu-container').innerHTML = '<p style="text-align:center;color:var(--text-secondary);margin-top:2rem;">No hay categorías disponibles.</p>';
        }

        // Show UI
        document.getElementById('loading').classList.add('hidden');
        document.getElementById('restaurant-header').classList.remove('hidden');
        document.getElementById('category-tabs').classList.remove('hidden');
        document.getElementById('menu-container').classList.remove('hidden');

    } catch (err) {
        console.error(err);
        document.getElementById('loading').classList.add('hidden');
        document.getElementById('error').classList.remove('hidden');
    }

    // Modal Close logic
    const modal = document.getElementById('item-modal');
    const overlay = document.querySelector('.modal-overlay');
    const closeBtn = document.getElementById('modal-close');
    
    const closeModal = () => {
        modal.classList.remove('active');
        setTimeout(() => modal.classList.add('hidden'), 300); // match transition
    };

    overlay.addEventListener('click', closeModal);
    closeBtn.addEventListener('click', closeModal);
});

function renderTabs(categories, slug, apiPrefix) {
    const tabsContainer = document.getElementById('category-tabs');
    tabsContainer.innerHTML = '';
    
    categories.forEach((cat, index) => {
        const btn = document.createElement('button');
        btn.className = `tab-btn ${index === 0 ? 'active' : ''}`;
        btn.textContent = cat.name;
        btn.onclick = async () => {
            document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            btn.scrollIntoView({ behavior: 'smooth', inline: 'center', block: 'nearest' });
            await loadCategoryItems(slug, cat.id, apiPrefix);
        };
        tabsContainer.appendChild(btn);
    });
}

const formatPrice = (p) => new Intl.NumberFormat('es-MX', { style: 'currency', currency: 'MXN' }).format(p);

async function loadCategoryItems(slug, categoryId, apiPrefix) {
    const container = document.getElementById('menu-container');
    
    // Fade out animation
    container.style.opacity = '0';
    
    await new Promise(r => setTimeout(r, 150)); // Wait for fade out

    container.innerHTML = '<div style="text-align:center;padding:2rem;">Cargando platillos...</div>';
    container.style.opacity = '1';

    try {
        const response = await fetch(`${API_BASE}${apiPrefix}/${slug}/items/${categoryId}`);
        const items = await response.json();

        container.style.opacity = '0';
        await new Promise(r => setTimeout(r, 150));

        container.innerHTML = '';
        
        if (items.length === 0) {
            container.innerHTML = '<p style="text-align:center;color:var(--text-secondary);margin-top:2rem;">Esta categoría no tiene platillos.</p>';
            container.style.opacity = '1';
            return;
        }

        items.forEach(item => {
            const hasPromo = item.isPromotion && item.discountPercent > 0;
            const finalPrice = hasPromo 
                ? (item.price - (item.price * (item.discountPercent / 100))) 
                : item.price;
                
            const card = document.createElement('div');
            card.className = 'menu-card';
            
            // Attach click event for Modal
            card.onclick = () => openModal(item, finalPrice, hasPromo);
            
            let html = '';
            
            if (item.photoUrl) {
                html += `<img src="${item.photoUrl}" alt="${item.name}" class="menu-img">`;
            }
            
            if (hasPromo) {
                html += `<div class="badge-promo">-${item.discountPercent}%</div>`;
            }
            
            html += `<div class="menu-details">
                <h3 class="menu-title">${item.name}</h3>
                <p class="menu-desc">${item.description}</p>
                <div class="price-row">
                    ${hasPromo ? `<span class="original-price">${formatPrice(item.price)}</span>` : ''}
                    <span class="menu-price">${formatPrice(finalPrice)}</span>
                </div>
            </div>`;
            
            card.innerHTML = html;
            container.appendChild(card);
        });
        
        // Fade in animation
        container.style.opacity = '1';

    } catch (err) {
        console.error('Error loading items:', err);
        container.innerHTML = '<div class="error-msg" style="padding:1rem;">Error al cargar platillos</div>';
        container.style.opacity = '1';
    }
}

function openModal(item, finalPrice, hasPromo) {
    const modal = document.getElementById('item-modal');
    
    document.getElementById('modal-img').src = item.photoUrl || '';
    document.getElementById('modal-img').style.display = item.photoUrl ? 'block' : 'none';
    
    document.getElementById('modal-title').textContent = item.name;
    document.getElementById('modal-desc').textContent = item.description;
    
    const priceContainer = document.getElementById('modal-price-container');
    priceContainer.innerHTML = '';
    
    if (hasPromo) {
        priceContainer.innerHTML += `<span class="original-price" style="font-size:1rem;">${formatPrice(item.price)}</span>`;
    }
    priceContainer.innerHTML += `<span>${formatPrice(finalPrice)}</span>`;

    modal.classList.remove('hidden');
    // Force reflow
    void modal.offsetWidth;
    modal.classList.add('active');
}
