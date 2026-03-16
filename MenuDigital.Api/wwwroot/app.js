const API_BASE = window.location.origin;

document.addEventListener('DOMContentLoaded', async () => {
    // Determine slug from URL, e.g. domain.com/demo
    // Since static files serve index.html, the path might just be /demo
    // Let's grab the slug from pathname
    let slug = window.location.pathname.replace(/^\/|\/$/g, '');
    
    // If empty or index.html, fallback to demo slug for easy testing
    if (!slug || slug.includes('index.html')) {
        slug = 'demo';
    }

    try {
        // 1. Fetch Restaurant Info
        const resResponse = await fetch(`${API_BASE}/${slug}/restaurant`);
        if (!resResponse.ok) throw new Error('Restaurante no encontrado');
        const restaurant = await resResponse.json();

        document.getElementById('logo').src = restaurant.logoUrl;
        document.getElementById('restaurant-name').textContent = restaurant.name;
        document.getElementById('restaurant-address').textContent = restaurant.address;

        // 2. Fetch Categories
        const catResponse = await fetch(`${API_BASE}/${slug}/categories`);
        const categories = await catResponse.json();
        
        if (categories.length > 0) {
            renderTabs(categories, slug);
            // Auto-load first category items
            await loadCategoryItems(slug, categories[0].id);
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
});

function renderTabs(categories, slug) {
    const tabsContainer = document.getElementById('category-tabs');
    tabsContainer.innerHTML = '';
    
    categories.forEach((cat, index) => {
        const btn = document.createElement('button');
        btn.className = `tab-btn ${index === 0 ? 'active' : ''}`;
        btn.textContent = cat.name;
        btn.onclick = async () => {
            // Update active state
            document.querySelectorAll('.tab-btn').forEach(b => b.classList.remove('active'));
            btn.classList.add('active');
            // Scroll to center tab
            btn.scrollIntoView({ behavior: 'smooth', inline: 'center', block: 'nearest' });
            // Load items
            await loadCategoryItems(slug, cat.id);
        };
        tabsContainer.appendChild(btn);
    });
}

async function loadCategoryItems(slug, categoryId) {
    const container = document.getElementById('menu-container');
    container.innerHTML = '<div style="text-align:center;padding:2rem;">Cargando platillos...</div>';

    try {
        const response = await fetch(`${API_BASE}/${slug}/items/${categoryId}`);
        const items = await response.json();

        container.innerHTML = '';
        
        if (items.length === 0) {
            container.innerHTML = '<p style="text-align:center;color:var(--text-secondary);margin-top:2rem;">Esta categoría no tiene platillos.</p>';
            return;
        }

        items.forEach(item => {
            const hasPromo = item.isPromotion && item.discountPercent > 0;
            const finalPrice = hasPromo 
                ? (item.price - (item.price * (item.discountPercent / 100))) 
                : item.price;
                
            const formatPrice = (p) => new Intl.NumberFormat('es-MX', { style: 'currency', currency: 'MXN' }).format(p);

            const card = document.createElement('div');
            card.className = 'menu-card';
            
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
                <div class="menu-price">
                    ${hasPromo ? `<span class="original-price">${formatPrice(item.price)}</span>` : ''}
                    <span>${formatPrice(finalPrice)}</span>
                </div>
            </div>`;
            
            card.innerHTML = html;
            container.appendChild(card);
        });

    } catch (err) {
        console.error('Error loading items:', err);
        container.innerHTML = '<div class="error-msg" style="padding:1rem;">Error al cargar platillos</div>';
    }
}
