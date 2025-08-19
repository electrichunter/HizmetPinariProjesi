# ***************************************************************
# DOSYA: usertest.py (Cookie Uyumlu)
# AÇIKLAMA: HizmetPınarı API'sinin Cookie tabanlı kayıt ve giriş
#           fonksiyonlarını test eden Python scripti.
# ÇALIŞTIRMA: python test/usertest.py
# ***************************************************************

import requests
import json
import time

# --- AYARLAR ---
BASE_URL = "http://localhost:5064/api"
ADMIN_KEY = "SECRET_ADMIN_KEY_123"
SUPPORT_KEY = "SECRET_SUPPORT_KEY_456"

# --- TEST VERİLERİ ---
timestamp = int(time.time())
admin_email = f"admin_{timestamp}@hizmetpinari.com"
admin_password = "AdminPassword123"

admin_data = {
    "email": admin_email,
    "password": admin_password,
    "firstName": "Test",
    "lastName": "Admin",
    "adminKey": ADMIN_KEY
}

def print_status(step, response):
    """Test adımlarının sonucunu terminale yazdırır."""
    if response is None:
        return
        
    status_code = response.status_code
    if 200 <= status_code < 300:
        print(f"✅ {step}")
    else:
        print(f"❌ {step} - BAŞARISIZ (HTTP Durum Kodu: {status_code})")
        print(f"   └── Hata Detayı: {response.text}\n")

def make_request(session, method, url, **kwargs):
    """İstekleri yapar ve olası bağlantı hatalarını yakalar."""
    try:
        response = session.request(method, url, timeout=10, **kwargs)
        return response
    except requests.exceptions.RequestException as e:
        print(f"❌ BAĞLANTI HATASI - API çalışmıyor veya {url} adresine ulaşılamıyor.")
        print(f"   └── Sistem Hatası: {e}\n")
        return None

def run_tests():
    """Tüm test senaryolarını sırasıyla çalıştırır."""
    # Session objesi, çerezleri otomatik olarak yönetir.
    session = requests.Session()
    
    print("🚀 API Testleri Başlatılıyor (Cookie Modu)...\n" + "="*45)
    
    # --- TEST 1: Başarılı Admin Kaydı ---
    response = make_request(session, "post", f"{BASE_URL}/admin/register", json=admin_data)
    print_status("Admin Kaydı", response)

    # --- TEST 2: Korumalı Alana GİRİŞ YAPMADAN Erişme (Hata Bekleniyor) ---
    response = make_request(session, "get", f"{BASE_URL}/auth/me")
    print_status("Korumalı Alana GİRİŞ YAPMADAN Erişme (401 Unauthorized bekleniyor)", response)

    # --- TEST 3: Başarılı Giriş (Login) ---
    login_data = {"email": admin_email, "password": admin_password}
    response = make_request(session, "post", f"{BASE_URL}/auth/login", json=login_data)
    print_status("Giriş Yapma (Login)", response)
    # Bu adımdan sonra session objesi, sunucudan gelen çerezi saklar.

    # --- TEST 4: Korumalı Alana GİRİŞ YAPTIKTAN SONRA Erişme ---
    response = make_request(session, "get", f"{BASE_URL}/auth/me")
    print_status("Korumalı Alana GİRİŞ YAPTIKTAN SONRA Erişme", response)

    # --- TEST 5: Başarılı Çıkış (Logout) ---
    response = make_request(session, "post", f"{BASE_URL}/auth/logout")
    print_status("Çıkış Yapma (Logout)", response)
    # Bu adımdan sonra session objesindeki çerez geçersiz hale gelir.

    # --- TEST 6: Korumalı Alana ÇIKIŞ YAPTIKTAN SONRA Erişme (Hata Bekleniyor) ---
    response = make_request(session, "get", f"{BASE_URL}/auth/me")
    print_status("Korumalı Alana ÇIKIŞ YAPTIKTAN SONRA Erişme (401 Unauthorized bekleniyor)", response)
    
    print("="*45 + "\n🏁 Testler Tamamlandı.")


if __name__ == "__main__":
    run_tests()
