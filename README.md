# Pazar Yeri Projesi - Faz 1 (MVP) Görev Listesi

Bu belge, pazar yeri uygulamamızın ilk geliştirme fazı olan **MVP (Minimum Viable Product)** için tamamlanması gereken görevleri listelemektedir. Görevler tamamlandıkça yanlarındaki kutucuklar işaretlenerek projenin genel ilerlemesi takip edilebilir.

---

## Faz 1: Temel Pazar Yeri İşlevselliği (MVP)

### Müşteri Kayıt ve Profil Sistemi

- [1] **Backend:** `CustomerController.cs` oluştur
- [1] **Backend:** Müşteri kayıt endpoint'i oluştur
- [] **Frontend:** Müşteri kayıt ekranını tasarla
- [] **Frontend:** Kayıt API entegrasyonunu yap
- [] **Frontend:** Profil ekranını güncelle

### Hizmet Talebi Oluşturma (Müşteri)

- [ ] **Backend:** `ServiceRequest.cs` modelini oluştur
- [ ] **Backend:** `CreateRequestDto.cs` ve `RequestDetailsDto.cs` oluştur
- [ ] **Backend:** `ServiceRequestController.cs` oluştur
- [ ] **Backend:** Talep oluşturma endpoint'i ekle
- [ ] **Backend:** "Taleplerim" endpoint'i ekle
- [ ] **Frontend:** Menüye "Talep Oluştur" ve "Taleplerim" ekle
- [ ] **Frontend:** `CreateRequestScreen.tsx` ekranını oluştur
- [ ] **Frontend:** `MyRequestsScreen.tsx` ekranını oluştur

### Açık İşleri Görüntüleme (Hizmet Veren)

- [ ] **Backend:** "Açık İşler" endpoint'i ekle
- [ ] **Frontend:** `ProviderStack.tsx` menüsünü oluştur
- [ ] **Frontend:** `AppNavigator.tsx` güncelle
- [ ] **Frontend:** `OpenRequestsScreen.tsx` ekranını oluştur

### Teklif Verme ve Değerlendirme

- [ ] **Backend:** `Offer.cs` modeli ve DTO'ları oluştur
- [ ] **Backend:** `OfferController.cs` oluştur
- [ ] **Backend:** Teklif verme endpoint'i oluştur
- [ ] **Backend:** Teklifleri listeleme endpoint'i oluştur
- [ ] **Backend:** Teklif kabul etme endpoint'i oluştur
- [ ] **Frontend:** `RequestDetailScreen.tsx` ekranını oluştur
- [ ] **Frontend:** Teklif verme formunu oluştur
- [ ] **Frontend:** `RequestOffersScreen.tsx` ekranını oluştur
