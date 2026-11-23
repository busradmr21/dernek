from docx import Document
from docx.enum.section import WD_SECTION
from docx.enum.table import WD_CELL_VERTICAL_ALIGNMENT, WD_ROW_HEIGHT_RULE, WD_TABLE_ALIGNMENT
from docx.enum.text import WD_ALIGN_PARAGRAPH, WD_BREAK, WD_LINE_SPACING
from docx.oxml import OxmlElement
from docx.oxml.ns import qn
from docx.shared import Cm, Inches, Pt, RGBColor
from pathlib import Path
from datetime import date


ROOT = Path(r"C:\Users\busra\source\repos\SigortaDernegi")
OUTPUT = ROOT / "docs" / "HISAD_Yonetim_Rolleri_ve_Admin_Paneli_Yetki_Raporu.docx"
LOGO = ROOT / "SigortaDernegi" / "wwwroot" / "images" / "hisad-logo-mark.png"

NAVY = "0A2540"
NAVY_2 = "123E5E"
TEAL = "008F86"
PALE_BLUE = "EEF5F9"
PALE_TEAL = "E7F7F5"
LIGHT_GRAY = "F5F7F9"
BORDER = "D9E1E7"
TEXT = "1B2B3A"
MUTED = "5F7182"
WHITE = "FFFFFF"
BLACK = "000000"


def set_cell_fill(cell, color):
    tc_pr = cell._tc.get_or_add_tcPr()
    shading = tc_pr.find(qn("w:shd"))
    if shading is None:
        shading = OxmlElement("w:shd")
        tc_pr.append(shading)
    shading.set(qn("w:fill"), color)


def set_cell_border(cell, color=BORDER, size="6"):
    tc_pr = cell._tc.get_or_add_tcPr()
    borders = tc_pr.first_child_found_in("w:tcBorders")
    if borders is None:
        borders = OxmlElement("w:tcBorders")
        tc_pr.append(borders)
    for edge in ("top", "left", "bottom", "right", "insideH", "insideV"):
        tag = "w:" + edge
        element = borders.find(qn(tag))
        if element is None:
            element = OxmlElement(tag)
            borders.append(element)
        element.set(qn("w:val"), "single")
        element.set(qn("w:sz"), size)
        element.set(qn("w:color"), color)


def set_cell_margins(cell, top=110, start=120, bottom=110, end=120):
    tc = cell._tc
    tc_pr = tc.get_or_add_tcPr()
    tc_mar = tc_pr.first_child_found_in("w:tcMar")
    if tc_mar is None:
        tc_mar = OxmlElement("w:tcMar")
        tc_pr.append(tc_mar)
    for margin, value in (("top", top), ("start", start), ("bottom", bottom), ("end", end)):
        node = tc_mar.find(qn(f"w:{margin}"))
        if node is None:
            node = OxmlElement(f"w:{margin}")
            tc_mar.append(node)
        node.set(qn("w:w"), str(value))
        node.set(qn("w:type"), "dxa")


def set_row_repeat(row):
    tr_pr = row._tr.get_or_add_trPr()
    tbl_header = OxmlElement("w:tblHeader")
    tbl_header.set(qn("w:val"), "true")
    tr_pr.append(tbl_header)


def prevent_row_split(row):
    tr_pr = row._tr.get_or_add_trPr()
    cant_split = OxmlElement("w:cantSplit")
    tr_pr.append(cant_split)


def keep_with_next(paragraph):
    p_pr = paragraph._p.get_or_add_pPr()
    keep_next = OxmlElement("w:keepNext")
    p_pr.append(keep_next)


def set_repeat_table_header(row):
    set_row_repeat(row)
    for cell in row.cells:
        cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER


def add_hyperlink(paragraph, text, url):
    part = paragraph.part
    relation_id = part.relate_to(
        url,
        "http://schemas.openxmlformats.org/officeDocument/2006/relationships/hyperlink",
        is_external=True,
    )
    hyperlink = OxmlElement("w:hyperlink")
    hyperlink.set(qn("r:id"), relation_id)
    run = OxmlElement("w:r")
    run_properties = OxmlElement("w:rPr")
    color = OxmlElement("w:color")
    color.set(qn("w:val"), TEAL)
    underline = OxmlElement("w:u")
    underline.set(qn("w:val"), "single")
    run_properties.append(color)
    run_properties.append(underline)
    run.append(run_properties)
    text_node = OxmlElement("w:t")
    text_node.text = text
    run.append(text_node)
    hyperlink.append(run)
    paragraph._p.append(hyperlink)
    return hyperlink


def add_page_number(paragraph):
    paragraph.alignment = WD_ALIGN_PARAGRAPH.RIGHT
    run = paragraph.add_run("Sayfa ")
    run.font.size = Pt(8.5)
    run.font.color.rgb = RGBColor.from_string(MUTED)
    fld_char1 = OxmlElement("w:fldChar")
    fld_char1.set(qn("w:fldCharType"), "begin")
    instr_text = OxmlElement("w:instrText")
    instr_text.set(qn("xml:space"), "preserve")
    instr_text.text = "PAGE"
    fld_char2 = OxmlElement("w:fldChar")
    fld_char2.set(qn("w:fldCharType"), "end")
    run._r.append(fld_char1)
    run._r.append(instr_text)
    run._r.append(fld_char2)


def set_font(run, name="Aptos", size=None, bold=None, color=None):
    run.font.name = name
    run._element.get_or_add_rPr().rFonts.set(qn("w:ascii"), name)
    run._element.get_or_add_rPr().rFonts.set(qn("w:hAnsi"), name)
    if size is not None:
        run.font.size = Pt(size)
    if bold is not None:
        run.bold = bold
    if color is not None:
        run.font.color.rgb = RGBColor.from_string(color)


def add_body(doc, text, bold_lead=None, after=6):
    paragraph = doc.add_paragraph()
    paragraph.paragraph_format.space_after = Pt(after)
    paragraph.paragraph_format.line_spacing = 1.15
    if bold_lead and text.startswith(bold_lead):
        lead = paragraph.add_run(bold_lead)
        set_font(lead, bold=True, color=TEXT)
        remainder = paragraph.add_run(text[len(bold_lead):])
        set_font(remainder, color=TEXT)
    else:
        run = paragraph.add_run(text)
        set_font(run, color=TEXT)
    return paragraph


def add_bullet(doc, text, level=0, bold_lead=None):
    paragraph = doc.add_paragraph(style="List Bullet" if level == 0 else "List Bullet 2")
    paragraph.paragraph_format.space_after = Pt(3)
    paragraph.paragraph_format.line_spacing = 1.08
    if bold_lead and text.startswith(bold_lead):
        first = paragraph.add_run(bold_lead)
        set_font(first, bold=True, color=TEXT)
        rest = paragraph.add_run(text[len(bold_lead):])
        set_font(rest, color=TEXT)
    else:
        set_font(paragraph.add_run(text), color=TEXT)
    return paragraph


def add_number(doc, text):
    paragraph = doc.add_paragraph(style="List Number")
    paragraph.paragraph_format.space_after = Pt(4)
    paragraph.paragraph_format.line_spacing = 1.1
    set_font(paragraph.add_run(text), color=TEXT)
    return paragraph


def add_heading(doc, text, level=1):
    paragraph = doc.add_heading(text, level=level)
    paragraph.paragraph_format.space_before = Pt(12 if level == 1 else 8)
    paragraph.paragraph_format.space_after = Pt(6)
    keep_with_next(paragraph)
    for run in paragraph.runs:
        set_font(run, size=16 if level == 1 else 12.5, bold=True, color=BLACK)
    return paragraph


def add_table(doc, headers, rows, widths=None, font_size=8.5, header_color=NAVY):
    table = doc.add_table(rows=1, cols=len(headers))
    table.alignment = WD_TABLE_ALIGNMENT.CENTER
    table.autofit = False
    table.style = "Table Grid"
    header = table.rows[0]
    set_repeat_table_header(header)
    for index, label in enumerate(headers):
        cell = header.cells[index]
        set_cell_fill(cell, header_color)
        set_cell_border(cell)
        set_cell_margins(cell, 125, 120, 125, 120)
        if widths:
            cell.width = Cm(widths[index])
        paragraph = cell.paragraphs[0]
        paragraph.alignment = WD_ALIGN_PARAGRAPH.LEFT if index == 0 else WD_ALIGN_PARAGRAPH.CENTER
        paragraph.paragraph_format.space_after = Pt(0)
        run = paragraph.add_run(label)
        set_font(run, size=8.3, bold=True, color=WHITE)

    for row_index, row_data in enumerate(rows):
        row = table.add_row()
        prevent_row_split(row)
        for index, value in enumerate(row_data):
            cell = row.cells[index]
            cell.vertical_alignment = WD_CELL_VERTICAL_ALIGNMENT.CENTER
            set_cell_border(cell)
            set_cell_margins(cell, 110, 120, 110, 120)
            if widths:
                cell.width = Cm(widths[index])
            if row_index % 2 == 1:
                set_cell_fill(cell, PALE_BLUE)
            paragraph = cell.paragraphs[0]
            paragraph.alignment = WD_ALIGN_PARAGRAPH.LEFT if index in (0, len(row_data) - 1) else WD_ALIGN_PARAGRAPH.CENTER
            paragraph.paragraph_format.space_after = Pt(0)
            paragraph.paragraph_format.line_spacing = 1.0
            run = paragraph.add_run(str(value))
            set_font(run, size=font_size, color=TEXT, bold=(index == 0))
    doc.add_paragraph().paragraph_format.space_after = Pt(1)
    return table


doc = Document()
doc.core_properties.title = "HİSAD Yönetim Rolleri ve Admin Paneli Yetki Raporu"
doc.core_properties.subject = "Yönetim kurulu rolleri için görev ve sistem yetkisi tasarımı"
doc.core_properties.author = "HİSAD"
doc.core_properties.keywords = "HİSAD, rol yönetimi, yetki matrisi, admin paneli, yönetim kurulu"

section = doc.sections[0]
section.top_margin = Cm(1.8)
section.bottom_margin = Cm(1.9)
section.left_margin = Cm(2.0)
section.right_margin = Cm(2.0)
section.footer_distance = Cm(0.7)

styles = doc.styles
normal = styles["Normal"]
normal.font.name = "Aptos"
normal._element.rPr.rFonts.set(qn("w:ascii"), "Aptos")
normal._element.rPr.rFonts.set(qn("w:hAnsi"), "Aptos")
normal.font.size = Pt(10)
normal.font.color.rgb = RGBColor.from_string(TEXT)

title_style = styles["Title"]
title_style.font.name = "Aptos Display"
title_style._element.rPr.rFonts.set(qn("w:ascii"), "Aptos Display")
title_style._element.rPr.rFonts.set(qn("w:hAnsi"), "Aptos Display")
title_style.font.size = Pt(29)
title_style.font.bold = True
title_style.font.color.rgb = RGBColor.from_string(BLACK)

for name, size in (("Heading 1", 16), ("Heading 2", 12.5), ("Heading 3", 11)):
    style = styles[name]
    style.font.name = "Aptos Display"
    style._element.rPr.rFonts.set(qn("w:ascii"), "Aptos Display")
    style._element.rPr.rFonts.set(qn("w:hAnsi"), "Aptos Display")
    style.font.size = Pt(size)
    style.font.bold = True
    style.font.color.rgb = RGBColor.from_string(BLACK)

for current_section in doc.sections:
    footer = current_section.footer
    paragraph = footer.paragraphs[0]
    add_page_number(paragraph)

# Cover page
cover = doc.add_paragraph()
cover.paragraph_format.space_after = Pt(20)
cover.alignment = WD_ALIGN_PARAGRAPH.LEFT
if LOGO.exists():
    cover.add_run().add_picture(str(LOGO), width=Cm(2.5))

label = doc.add_paragraph()
label.paragraph_format.space_before = Pt(42)
label.paragraph_format.space_after = Pt(12)
run = label.add_run("HİSAD YÖNETİM SİSTEMİ")
set_font(run, size=10, bold=True, color=TEAL)

title = doc.add_paragraph(style="Title")
title.paragraph_format.space_after = Pt(12)
title.add_run("Yönetim Rolleri ve Admin Paneli Yetki Raporu")

subtitle = doc.add_paragraph()
subtitle.paragraph_format.space_after = Pt(20)
run = subtitle.add_run("Sigorta araştırmacıları ve uzmanları derneği için görev, erişim ve onay modeli")
set_font(run, size=14, color=MUTED)

intro = doc.add_paragraph()
intro.paragraph_format.space_before = Pt(18)
intro.paragraph_format.space_after = Pt(10)
intro.paragraph_format.line_spacing = 1.22
run = intro.add_run(
    "Bu rapor, dernek başkanı, başkan yardımcısı, genel sekreter ve saymanın yönetim panelinde "
    "hangi bilgileri görebileceğini ve hangi işlemleri yapabileceğini tanımlar. Önerilen model, görevlerin "
    "ayrılığı, kişisel verilerin korunması, finansal kontrol ve işlem izlenebilirliği esaslarına dayanır."
)
set_font(run, size=11, color=TEXT)

meta = doc.add_paragraph()
meta.paragraph_format.space_before = Pt(80)
meta.paragraph_format.space_after = Pt(0)
set_font(meta.add_run("Hazırlanma tarihi  "), size=9, bold=True, color=MUTED)
set_font(meta.add_run("15 Ekim 2026"), size=9, color=TEXT)

meta2 = doc.add_paragraph()
meta2.paragraph_format.space_after = Pt(0)
set_font(meta2.add_run("Belge amacı  "), size=9, bold=True, color=MUTED)
set_font(meta2.add_run("Proje geliştirme ve yetkilendirme taslağı"), size=9, color=TEXT)

doc.add_page_break()

add_heading(doc, "Yönetici Özeti", 1)
add_body(
    doc,
    "Önerilen modelde dernek başkanı Ana Yönetici rolünü taşır ve sistemin tamamından sorumludur. "
    "Başkan yardımcısı operasyonları ve içerikleri geniş ölçüde izler, ancak rol atama, sistem ayarları, "
    "PayTR gizli bilgileri ve kalıcı veri işlemlerine erişmez. Genel sekreter üyelik, evrak, toplantı, "
    "iletişim ve içerik süreçlerini yürütür. Sayman ise aidat, PayTR, bağış, gelir gider ve mali raporlarla "
    "sınırlandırılmış finans rolünü kullanır."
)
add_body(
    doc,
    "Üyelik kabulü, yönetim kurulu kararlarının kesinleştirilmesi, PayTR iadesi, kişisel veri dışa aktarımı, "
    "rol değişikliği ve kayıtların kalıcı silinmesi tek kullanıcının kontrolünde bırakılmamalıdır. Sistem, bu "
    "işlemleri karar kaydı veya ikinci onay ile ilişkilendirmeli ve her değişikliği denetim günlüğüne yazmalıdır."
)

add_heading(doc, "Temel Kararlar", 2)
for item in [
    "Admin paneli menüleri rol bazında gizlenmeli; yetkisiz alan yalnızca düğme gizlenerek değil sunucu tarafında da engellenmelidir.",
    "Dernek görevi ile sistem rolü ayrı tutulmalıdır. Başkan yardımcısı olmak otomatik olarak tüm admin yetkilerini vermez.",
    "Ana Yönetici rolü yalnızca dernek başkanına atanmalı; bu rolü atama veya kaldırma işlemi kayda alınmalıdır.",
    "Resmî kararlar, üyelik kabulleri ve mali onaylar ilgili yönetim kurulu kararına bağlanmalıdır.",
    "Kalıcı silme yerine arşivleme kullanılmalı; silme zorunluysa işlem gerekçesi ve onayı tutulmalıdır.",
    "PayTR anahtarları hiçbir kullanıcıya ekranda gösterilmemeli; yalnızca sunucu ortamında güvenli biçimde saklanmalıdır.",
]:
    add_bullet(doc, item)

add_heading(doc, "Raporun Kapsamı", 2)
add_body(
    doc,
    "Rapor; üyelik ve uzman rehberi, yönetim kurulu kayıtları, aidat ve ödeme yönetimi, bağışlar, dernek "
    "defterleri, araştırma ve yayınlar, etkinlikler, site içerikleri, başvurular, iletişim, raporlar ve sistem "
    "ayarları için önerilen erişim modelini kapsar. Tüzükte veya yönetim kurulu kararlarında farklı bir görev "
    "dağılımı bulunması hâlinde yazılım yetkileri buna göre uyarlanmalıdır."
)

add_heading(doc, "Hukuki ve Yönetsel Çerçeve", 1)
add_body(
    doc,
    "Türk Medeni Kanununun 85 inci maddesine göre yönetim kurulu derneğin yürütme ve temsil organıdır ve "
    "görevini kanun ile dernek tüzüğüne uygun yürütür. Temsil görevi yönetim kurulu kararıyla bir üyeye veya "
    "üçüncü kişiye verilebilir. Bu nedenle yazılımdaki bireysel yetkiler, yönetim kurulunun kolektif karar "
    "sorumluluğunun yerine geçmez."
)
add_body(
    doc,
    "Üyelik başvuruları yönetim kurulunca görüşülerek en çok otuz gün içinde kabul veya ret şeklinde karara "
    "bağlanmalı ve sonuç başvuru sahibine yazılı olarak bildirilmelidir. Yazılımda genel sekreter başvuruyu "
    "hazırlayabilir ve karar bilgilerini girebilir; üyeliğin kesinleştirilmesi karar tarih ve numarasıyla yapılmalıdır."
)
add_body(
    doc,
    "Derneklerin işletme hesabı esasına göre karar, üye kayıt, evrak kayıt ve işletme hesabı defterlerini; "
    "bilanço esasına göre ise bunlara uygun mali defterleri tutması gerekir. Uygulamadaki defter modülü operasyonel "
    "kayıt ve çıktı üretme aracı olarak tasarlanmalı; resmî tasdik, imza, saklama ve DERBİS süreçleri ayrıca kontrol edilmelidir."
)
add_body(
    doc,
    "Bu rapordaki görev dağılımı yazılım ve iç kontrol önerisidir. Dernek tüzüğü, yönetim kurulu kararları ve "
    "güncel mevzuat esas alınmalı; mali ve hukuki uygulamalar yetkili mali müşavir veya hukuk danışmanıyla doğrulanmalıdır."
)

add_heading(doc, "Yetkilendirme İlkeleri", 1)
principles = [
    ("Asgari yetki", "Her kullanıcı yalnızca görevini yapmak için gereken alanlara erişir."),
    ("Görevlerin ayrılığı", "Kaydı oluşturan kişi ile kritik işlemi onaylayan kişi mümkün olduğunca farklı olur."),
    ("Varsayılan ret", "Yeni rol veya modül eklendiğinde açıkça izin verilmedikçe erişim kapalı kalır."),
    ("İşlem izlenebilirliği", "Ekleme, düzenleme, onay, dışa aktarma, iade ve arşiv işlemleri kullanıcı ve zaman bilgisiyle kaydedilir."),
    ("Karara bağlı yetki", "Üyelik kabulü, bütçe, iade ve resmî karar işlemleri ilgili kurul karar numarasıyla ilişkilendirilir."),
    ("Veri minimizasyonu", "Sayman yalnızca ödeme için gereken üye bilgilerini; içerik yöneticisi yalnızca yayımlanabilir profil bilgilerini görür."),
    ("Süreli vekâlet", "Başkanın yokluğunda verilen geçici yetkinin başlangıç ve bitiş tarihi bulunur ve süre sonunda otomatik kapanır."),
    ("Arşivleme", "Üye, ödeme, karar ve evrak kayıtlarında fiziksel silme yerine pasife alma veya arşivleme tercih edilir."),
]
for title_text, explanation in principles:
    add_bullet(doc, f"{title_text}: {explanation}", bold_lead=f"{title_text}: ")

add_heading(doc, "Rol Tanımları ve Genel Görev Alanları", 1)

add_heading(doc, "Dernek Başkanı ve Ana Yönetici", 2)
add_body(
    doc,
    "Dernek başkanı, yönetim kurulunun temsil ve yürütme sorumluluğunu sistemde Ana Yönetici rolüyle kullanır. "
    "Tüm modülleri görebilir; ancak kurul kararı gerektiren işlemleri tek başına karar üretmiş gibi kesinleştirmemelidir."
)
for item in [
    "Kullanıcı hesabı açar, yönetim kurulu üyesini sisteme davet eder ve rol atar.",
    "Rol şablonlarını ve özel kullanıcı yetkilerini belirler; süreli vekâlet tanımlar.",
    "Üyelik kararlarını kurul karar tarih ve numarasıyla kesinleştirir.",
    "Haber, araştırma raporu, mevzuat içeriği ve kamuya açık duyurular için son yayın onayını verir.",
    "Aidat dönemi, tutar ve ödeme politikasını yönetim kurulu kararı doğrultusunda onaylar.",
    "PayTR iade talebi, olağan dışı finansal düzeltme ve dönem kapatma işlemlerini onaylar.",
    "Dernek defterleri ve karar kayıtlarında kesinleştirme işlemini yapar; kilitlenen kaydı doğrudan değiştirmek yerine düzeltme kaydı açar.",
    "Denetim günlüğünü, kişisel veri dışa aktarımlarını ve güvenlik olaylarını inceler.",
]:
    add_bullet(doc, item)
add_body(doc, "Sınırlama: PayTR merchant key ve merchant salt değerleri ekranda gösterilmez; Ana Yönetici dahi yalnızca yapılandırmanın aktif olup olmadığını görür.", bold_lead="Sınırlama: ")

add_heading(doc, "Dernek Başkan Yardımcısı", 2)
add_body(
    doc,
    "Başkan yardımcısı, faaliyetlerin ve uzmanlık çalışmalarının sürekliliğini sağlar. Geniş görüntüleme ve operasyon "
    "yetkisine sahip olabilir; fakat sistem yönetimi ile kritik mali işlemlerde son onay yetkisi varsayılan olarak kapalıdır."
)
for item in [
    "Üye, uzman, komisyon ve yönetim kurulu profillerini görüntüler; görevlendirilmişse taslak düzenler.",
    "Sigorta araştırmaları, saha inceleme standartları, etik ilkeler ve sektörel yayın süreçlerini koordine eder.",
    "Komisyon ve çalışma grubu kayıtlarını, toplantı takvimini ve etkinlik programını yönetir.",
    "Haber, duyuru, mevzuat ve araştırma içeriklerini inceleyerek yayın onayına gönderir veya yetki verilmişse yayımlar.",
    "Aidat ve bütçe özetlerini görüntüler; ödeme kaydı, iade veya banka mutabakatı değiştiremez.",
    "Başkanın süreli vekâlet vermesi hâlinde belirlenen işlemleri son tarihe kadar onaylayabilir.",
]:
    add_bullet(doc, item)
add_body(doc, "Varsayılan yasaklar: rol atama, kullanıcı silme, PayTR ayarı, iade onayı, mali dönem kapatma, kalıcı silme ve tam kişisel veri dışa aktarma.", bold_lead="Varsayılan yasaklar: ")

add_heading(doc, "Genel Sekreter", 2)
add_body(
    doc,
    "Genel sekreter derneğin idari hafızasını, yazışmalarını, toplantı hazırlıklarını ve üyelik işlem akışını yönetir. "
    "Bu rol, kayıt oluşturma ve düzenlemede güçlü; mali onay ve sistem yönetiminde sınırlı olmalıdır."
)
for item in [
    "Üyelik başvurularını kontrol eder, eksik bilgi ister ve yönetim kurulu gündemine hazırlar.",
    "Kurul kararından sonra kabul veya ret bilgisini karar tarih ve numarasıyla sisteme işler.",
    "Üye ve uzman profillerini oluşturur; yayın izni, iletişim görünürlüğü ve belge durumunu takip eder.",
    "Yönetim kurulu toplantı gündemini hazırlar, karar taslağı oluşturur, katılımcı ve ek belgeleri kaydeder.",
    "Gelen ve giden evrak kayıtlarını, resmî yazışmaları ve dosya arşivini yönetir.",
    "İletişim mesajları, gönüllü başvuruları, etkinlik kayıtları ve kurumsal takvimi koordine eder.",
    "Haber ve duyuru taslağı hazırlayabilir; kamuya açık yayın son onayı yetkisi ayrıca verilmelidir.",
    "Aidat borcunun varlığını görebilir; ödeme ayrıntıları ve mali raporları değiştiremez.",
]:
    add_bullet(doc, item)
add_body(doc, "Varsayılan yasaklar: rol yönetimi, sistem ayarları, PayTR işlem/iade yönetimi, gelir gider kaydı onayı, mali rapor kapatma ve kalıcı silme.", bold_lead="Varsayılan yasaklar: ")

add_heading(doc, "Sayman", 2)
add_body(
    doc,
    "Sayman, aidat ve diğer gelirlerin izlenmesi, gider belgelerinin kaydı, PayTR mutabakatı ve mali raporlamadan sorumludur. "
    "Üye bilgilerinde yalnızca ödeme işlemi için gerekli alanları görmelidir."
)
for item in [
    "Aidat dönemlerini ve üye borçlarını yönetim kurulu kararına göre hazırlar.",
    "PayTR ödeme, başarısız işlem, iade ve mutabakat kayıtlarını izler.",
    "Banka, nakit ve diğer tahsilat kayıtlarını belge numarasıyla işler.",
    "Bağış kayıtlarını, gelir gider belgelerini ve ödeme durumlarını düzenler.",
    "Aylık ve yıllık mali özet, aidat borçluları, tahsilat ve bütçe gerçekleşme raporlarını hazırlar.",
    "İade veya olağan dışı düzeltme talebi oluşturur; başkan onayından sonra işlem uygulanır.",
    "İşletme hesabı kayıtlarını hazırlar ve dönem sonu raporunu onaya sunar.",
]:
    add_bullet(doc, item)
add_body(doc, "Varsayılan yasaklar: üye profilini değiştirme, üyelik kabulü, içerik yayımlama, yönetim kurulu kararını kesinleştirme, rol atama ve sistem ayarlarına erişme.", bold_lead="Varsayılan yasaklar: ")

add_heading(doc, "Rol Bazlı Admin Paneli Görünümü", 1)
add_body(doc, "Aşağıdaki menü önerisi, kullanıcının yalnızca görev alanıyla ilgili bölümleri görmesini sağlar. Yetkisiz modüller pasif gösterilmek yerine tamamen gizlenmelidir.")

menu_rows = [
    ("Dernek Başkanı", "Tüm modüller", "Rol ve Yetkiler, Sistem Ayarları, Denetim Günlüğü, onay bekleyen kritik işlemler"),
    ("Başkan Yardımcısı", "Ana Panel, Üyeler, Yönetim Kurulu, Araştırmalar, Komisyonlar, Haberler, Etkinlikler, Raporlar", "Finans yalnızca özet; Sistem Ayarları ve Rol Yönetimi gizli"),
    ("Genel Sekreter", "Ana Panel, Üyelik Başvuruları, Üye Yönetimi, Yönetim Kurulu, Evrak, İletişim, Etkinlikler, İçerik Taslakları", "PayTR, gelir gider, rol yönetimi ve güvenlik ayarları gizli"),
    ("Sayman", "Finans Paneli, Aidatlar, PayTR Ödemeleri, Bağışlar, Gelir Gider, Mali Raporlar, Üye Borç Görünümü", "İçerik, iletişim, üyelik kararı, rol yönetimi ve sistem ayarları gizli"),
]
add_table(doc, ["Rol", "Görünecek ana menüler", "Gizli veya sınırlı alanlar"], menu_rows, widths=[3.3, 7.1, 6.1], font_size=7.4)

add_heading(doc, "Yetki Seviyeleri", 2)
level_rows = [
    ("G", "Görüntüle", "Listeyi ve ayrıntıyı okuyabilir; veri değiştiremez."),
    ("H", "Hazırla", "Taslak kayıt oluşturabilir ve kendi taslağını düzenleyebilir."),
    ("D", "Düzenle", "Yetkili olduğu mevcut kayıtları değiştirebilir."),
    ("O", "Onayla", "Taslağı veya kritik işlemi kesinleştirebilir."),
    ("Y", "Yayımla", "İçeriği kamuya açık hâle getirebilir veya yayından kaldırabilir."),
    ("A", "Arşivle", "Kaydı görünür akıştan çıkarabilir; veri korunur."),
    ("X", "Erişim yok", "Menü ve işlem kullanıcıya gösterilmez."),
]
add_table(doc, ["Kod", "Seviye", "Açıklama"], level_rows, widths=[1.7, 3.2, 11.6], font_size=9)

add_heading(doc, "Modül Bazlı Yetki Matrisi", 1)
matrix_rows = [
    ("Ana panel ve kurumsal özet", "G", "G", "G", "G", "Her rol için görevine uygun göstergeler"),
    ("Üye ve uzman rehberi", "G H D O A", "G H D", "G H D", "G", "Yayın izni olmadan profil yayımlanmaz"),
    ("Üyelik başvuruları", "G O A", "G", "G H D", "G", "Kabul kurul kararına dayanır"),
    ("Yönetim kurulu profilleri", "G H D O A", "G H D", "G H D", "G", "Görev dönemi ve görünürlük kaydı"),
    ("Rol ve kullanıcı yetkileri", "G H D O A", "X", "X", "X", "Yalnızca Ana Yönetici"),
    ("Komisyonlar ve çalışma grupları", "G H D O A", "G H D O", "G H D", "G", "Sigorta araştırma komisyonları dâhil"),
    ("Araştırma projeleri ve raporlar", "G H D O Y A", "G H D O Y", "G H D", "G", "Kamu yayını için son kontrol"),
    ("Mevzuat ve etik içerikleri", "G H D O Y A", "G H D O Y", "G H D", "G", "Hukuki içerikte uzman kontrolü önerilir"),
    ("Haberler ve duyurular", "G H D O Y A", "G H D O Y", "G H D", "G", "Sekreter varsayılan olarak taslak hazırlar"),
    ("Etkinlikler ve eğitimler", "G H D O Y A", "G H D O Y", "G H D", "G", "Katılımcı verileri rol kapsamıyla sınırlı"),
    ("Multimedya", "G H D O Y A", "G H D O Y", "G H D", "X", "Telif ve yayın izni kontrolü"),
    ("İletişim mesajları", "G D O A", "G", "G H D O A", "X", "Hassas mesajlar konuya göre sınırlandırılır"),
    ("Gönüllü başvuruları", "G D O A", "G", "G H D O A", "X", "Başvuru ve KVKK kayıtları"),
    ("Aidat dönemleri", "G O A", "G", "G", "G H D", "Tutar ve dönem kararı kurul kararına bağlanır"),
    ("Üye aidat borçları", "G O", "G", "G", "G H D", "Sayman borç ve tahsilatı işler"),
    ("PayTR ödemeleri", "G O", "G", "X", "G H D", "Kesin başarı yalnızca PayTR bildirimiyle"),
    ("PayTR iadeleri", "G O", "X", "X", "G H", "Sayman talep eder, Başkan onaylar"),
    ("Bağış kayıtları", "G O A", "G", "X", "G H D", "Düzeltmeler iz bırakır"),
    ("Gelir gider kayıtları", "G O A", "G", "X", "G H D", "Belge numarası ve ek dosya zorunlu"),
    ("Karar defteri hazırlığı", "G O A", "G H", "G H D", "G", "Kesin kayıt toplantı kararına dayanır"),
    ("Evrak kayıtları", "G O A", "G", "G H D O", "G", "Gelen ve giden evrak sıra numarası"),
    ("İşletme hesabı ve mali rapor", "G O A", "G", "X", "G H D", "Dönem kapatma Başkan onayına sunulur"),
    ("Kişisel veri dışa aktarma", "G O", "X", "H", "X", "Amaç ve kapsam kaydı zorunlu"),
    ("Denetim günlüğü", "G", "G", "G", "G", "Her rol yalnızca kapsamındaki kayıtları görür"),
    ("Sistem ve güvenlik ayarları", "G H D O", "X", "X", "X", "Gizli anahtarlar arayüzde gösterilmez"),
]
add_table(doc, ["Modül", "Başkan", "Başkan Yrd", "Genel Sekreter", "Sayman", "Kontrol notu"], matrix_rows, widths=[4.1, 2.1, 2.2, 2.4, 2.0, 4.4], font_size=7.7)

add_heading(doc, "Sigorta Araştırmacıları Derneğine Özgü Yetki Alanları", 1)

domain_sections = [
    ("Araştırma ve İnceleme Projeleri", [
        "Başkan proje açılışını ve kamuya açıklanacak nihai raporu onaylar.",
        "Başkan yardımcısı proje ekiplerini, takvimi, metodolojiyi ve çalışma gruplarını yönetir.",
        "Genel sekreter toplantı, belge, izin ve yayın takibini yürütür.",
        "Sayman proje bütçesini ve belgeli giderleri izler; araştırma içeriğini değiştiremez.",
        "Kişisel veya dosya bazlı sigorta verileri mümkün olduğunca anonimleştirilir; ham veriye erişim proje bazında sınırlandırılır.",
    ]),
    ("Uzmanlık Alanları ve Uzman Rehberi", [
        "Uzmanlık alanları standart bir sözlükten seçilir; serbest metin yalnızca açıklama için kullanılır.",
        "Uzman doğrulama belgesi yöneticiye özel kalır; ziyaretçiye yalnızca doğrulanmış rozeti gösterilir.",
        "Genel sekreter profil ve yayın izinlerini yönetir, başkan tartışmalı doğrulama kararını sonuçlandırır.",
        "Sayman, aidat ilişkisi nedeniyle üye numarası ve ödeme durumunu görür; uzmanlık belgesini göremez.",
    ]),
    ("Etik ve Güvenli Bildirim Süreçleri", [
        "Etik bildirimler normal iletişim mesajlarından ayrı ve kısıtlı bir kutuda tutulur.",
        "İçeriğe varsayılan olarak yalnızca Başkan ve açıkça görevlendirilen Başkan Yardımcısı erişir.",
        "Genel sekreter bildirim geldi bilgisini ve süreç durumunu görebilir; hassas metin erişimi ayrıca verilir.",
        "Sayman, yalnızca mali usulsüzlük incelemesinde yönetim kurulu kararıyla sınırlı erişim alır.",
    ]),
    ("Mevzuat ve Sektörel Yayınlar", [
        "Taslak, teknik inceleme, hukuki kontrol ve yayın aşamaları ayrı durumlar olarak tutulur.",
        "Başkan yardımcısı ve uzman komisyonu teknik doğruluğu inceler; Başkan son yayın onayını verir.",
        "Genel sekreter kaynak, sürüm tarihi ve revizyon geçmişini kaydeder.",
        "Yayımlanan mevzuat içeriğinde yürürlük ve güncelleme tarihi açıkça gösterilir.",
    ]),
]
for heading, bullets in domain_sections:
    add_heading(doc, heading, 2)
    for item in bullets:
        add_bullet(doc, item)

add_heading(doc, "Kritik İşlem ve Onay Akışları", 1)
workflow_rows = [
    ("Üyeliğe kabul", "Genel Sekreter", "Yönetim kurulu kararı", "Başkan kesinleştirir", "Karar tarih ve numarası"),
    ("Uzman profilini yayımlama", "Genel Sekreter", "Başkan Yrd teknik inceleme", "Başkan veya yetkili Başkan Yrd", "Yayın izni ve doğrulama"),
    ("Aidat dönemi açma", "Sayman", "Başkan incelemesi", "Başkan", "Kurul kararı ve dönem bilgisi"),
    ("PayTR iadesi", "Sayman", "Tutar ve işlem doğrulaması", "Başkan", "İade nedeni ve PayTR referansı"),
    ("Gelir gider düzeltmesi", "Sayman", "Belge ve önceki kayıt kontrolü", "Başkan", "Düzeltme kaydı; eski kayıt korunur"),
    ("Yönetim kurulu kararı", "Genel Sekreter", "Toplantı katılımcıları", "Başkan", "Karar sayısı ve katılım bilgisi"),
    ("Kamuya açık araştırma raporu", "Başkan Yrd", "Uzman/komisyon kontrolü", "Başkan", "Sürüm ve kaynak geçmişi"),
    ("Rol veya yetki değişikliği", "Başkan", "Gerekirse kurul kararı", "Başkan", "Önceki ve yeni yetki kaydı"),
    ("Toplu kişisel veri aktarımı", "Genel Sekreter", "Amaç ve kapsam kontrolü", "Başkan", "Kim, ne zaman, hangi amaçla"),
]
add_table(doc, ["İşlem", "Hazırlayan", "Kontrol", "Son onay", "Zorunlu kayıt"], workflow_rows, widths=[3.3, 2.8, 3.5, 2.5, 4.8], font_size=8)

add_heading(doc, "Admin Paneli İçin Ekran Taslağı", 1)
add_body(doc, "Rol ve Yetki Yönetimi ekranı üç ana bölümden oluşmalıdır: rol şablonları, modül yetki matrisi ve kullanıcı rol atamaları.")

add_heading(doc, "Rol Şablonları", 2)
for item in [
    "Ana Yönetici rolü kilitli ve tüm yetkileri açık görünür.",
    "Başkan Yardımcısı, Genel Sekreter ve Sayman için önerilen başlangıç şablonları bulunur.",
    "Admin rol üzerinde değişiklik yaptığında etkilenecek kullanıcı sayısı gösterilir.",
    "Rol değişikliği kaydedilmeden önce eski ve yeni yetkiler özetlenir.",
]:
    add_bullet(doc, item)

add_heading(doc, "Yetki Matrisi", 2)
for item in [
    "Her modül için Görüntüle, Hazırla, Düzenle, Onayla, Yayımla, Arşivle ve Dışa Aktar izinleri ayrı seçilir.",
    "Kritik izinler farklı renkte ve açıklamayla gösterilir.",
    "Bir üst izin seçildiğinde gereken alt izinler sistem tarafından otomatik eklenir; örneğin Düzenle izni Görüntüle iznini gerektirir.",
    "Çakışan yetkiler için uyarı verilir; örneğin Sayman hem iade talebi oluşturup hem de aynı talebi onaylayamaz.",
]:
    add_bullet(doc, item)

add_heading(doc, "Kullanıcı Rol Atamaları", 2)
for item in [
    "Yönetim kurulu görevi ile panel rolü aynı satırda fakat ayrı alanlar olarak gösterilir.",
    "Kullanıcının aktif, davet bekliyor, askıya alınmış veya süresi dolmuş durumu bulunur.",
    "Rol dışında kullanıcıya özel ek izin verilecekse gerekçe ve bitiş tarihi zorunlu olur.",
    "Görev süresi sona erdiğinde panel erişimi otomatik askıya alınır; kayıtlar silinmez.",
]:
    add_bullet(doc, item)

add_heading(doc, "Menü ve Arayüz Davranışları", 2)
for item in [
    "Kullanıcının yetkisiz olduğu menüler gizlenir; doğrudan URL erişimi ayrıca engellenir.",
    "Salt okunur kullanıcıya düzenleme düğmesi gösterilmez.",
    "Onay bekleyen kayıtların sayısı rolün ana panelinde rozet olarak gösterilir.",
    "Kritik işlemlerde işlem özeti ve gerekçe alanı gösterilir.",
    "Başarılı işlem mesajı yapılan işlemi ve kayıt numarasını açıkça belirtir.",
    "Yetki yetersizliği durumunda genel hata yerine hangi yetkinin gerektiği açıklanır; hassas veri ayrıntısı verilmez.",
]:
    add_bullet(doc, item)

doc.add_page_break()
add_heading(doc, "Güvenlik ve Denetim Gereksinimleri", 1)
security_rows = [
    ("Kimlik doğrulama", "Her yöneticiye ayrı hesap; ortak kullanıcı hesabı kullanılmaz."),
    ("Rol kontrolü", "Hem ekran hem controller/action seviyesinde sunucu tarafı yetki kontrolü yapılır."),
    ("Oturum güvenliği", "Uzun süre işlem yapılmayan yönetici oturumu kapatılır; hassas işlemde yeniden doğrulama uygulanabilir."),
    ("Denetim günlüğü", "Kullanıcı, tarih, IP, işlem türü, kayıt kimliği ve önceki/yeni değer kaydedilir."),
    ("Kalıcı silme", "Varsayılan olarak kapalıdır; arşivleme kullanılır. İstisna işleminde gerekçe ve onay gerekir."),
    ("Dosya güvenliği", "Yüklenen belgeler tür, boyut ve zararlı içerik açısından kontrol edilir; doğrudan çalıştırılamaz."),
    ("Kişisel veriler", "Telefon, e-posta, kimlik ve belge alanları amaca göre maskelenir ve erişim kapsamı sınırlanır."),
    ("PayTR", "merchant_key ve merchant_salt kullanıcı arayüzünde gösterilmez; ödeme sonucu hash doğrulamasıyla kabul edilir."),
    ("Yedekleme", "Veritabanı ve yüklenen belgeler düzenli yedeklenir; geri yükleme denemesi periyodik yapılır."),
    ("Yetki gözden geçirme", "Roller ve özel izinler en az görev dönemi değişiminde ve düzenli aralıklarla kontrol edilir."),
]
add_table(doc, ["Kontrol alanı", "Gereksinim"], security_rows, widths=[4.1, 12.4], font_size=8.8)

add_heading(doc, "Denetim Günlüğünde Tutulacak İşlemler", 2)
for item in [
    "Giriş, başarısız giriş ve çıkış işlemleri",
    "Rol ekleme, rol değiştirme ve özel yetki verme",
    "Üye profili oluşturma, yayımlama, pasife alma ve dışa aktarma",
    "Üyelik başvurusunu kabul veya reddetme",
    "Aidat dönemi oluşturma ve borç düzeltme",
    "PayTR bildirimi, iade talebi, iade onayı ve mutabakat",
    "Gelir gider kaydı oluşturma, düzeltme ve dönem kapatma",
    "Karar ve evrak kayıtlarını kesinleştirme",
    "İçerik yayımlama, yayından kaldırma ve sürüm geri yükleme",
    "Arşivleme ve kalıcı silme girişimleri",
]:
    add_bullet(doc, item)

doc.add_page_break()
add_heading(doc, "Projeye Uygulama Yol Haritası", 1)
roadmap = [
    "MemberProfile modelini yönetim görevi ve Identity kullanıcı bağlantısı için genişletin.",
    "Yönetim kurulu görev dönemlerini ayrı BoardMembership kaydında tutun.",
    "Permission, RolePermission ve UserRoleAssignment yapılarını oluşturun.",
    "Ana Yönetici, Başkan Yardımcısı, Genel Sekreter ve Sayman başlangıç rol şablonlarını tanımlayın.",
    "Admin sol menüsünü rol izinlerine göre dinamik üretin.",
    "Controller ve action seviyesinde aynı izinleri zorunlu kılın.",
    "Rol ve Yetki Yönetimi ekranını hazırlayın.",
    "Kullanıcı daveti, rol atama, askıya alma ve süreli vekâlet akışlarını ekleyin.",
    "Üyelik ve uzman rehberini yönetim kurulu profilleriyle ilişkilendirin.",
    "Denetim günlüğünü kritik modüllere bağlayın.",
    "Yetki senaryolarını dört ayrı test hesabıyla doğrulayın.",
    "Üye altyapısı tamamlandıktan sonra PayTR online aidat yönetimine geçin.",
]
for item in roadmap:
    add_number(doc, item)

add_heading(doc, "Kabul Testleri", 2)
tests = [
    ("Başkan", "Rol ve Yetkiler ekranını açabilir, rol değiştirebilir ve değişiklik günlüğünü görebilir."),
    ("Başkan Yardımcısı", "Rol ve Yetkiler URL’sine doğrudan gitse bile erişemez; araştırma ve içerik alanına erişebilir."),
    ("Genel Sekreter", "Üyelik başvurusunu hazırlayabilir fakat kurul kararı olmadan üyeliği kesinleştiremez."),
    ("Sayman", "Aidat ve PayTR işlemlerini görebilir, iade talebi oluşturabilir fakat kendi talebini onaylayamaz."),
    ("Tüm roller", "Yetkisiz menüleri görmez ve yetkisiz API/controller çağrıları 403 sonucu verir."),
    ("Kayıt güvenliği", "Arşivlenen ödeme, karar veya üye kaydı denetim geçmişiyle birlikte korunur."),
]
add_table(doc, ["Senaryo", "Beklenen sonuç"], tests, widths=[4.2, 12.3], font_size=9)

add_heading(doc, "Önerilen İlk Uygulama Paketi", 1)
add_body(
    doc,
    "İlk geliştirme paketinde üye ve yönetim kurulu profilleri, dört rolün başlangıç şablonu, dinamik admin menüsü, "
    "temel yetki kontrolleri ve denetim günlüğü birlikte ele alınmalıdır. Bu altyapı tamamlanmadan PayTR, aidat ve "
    "dernek defterleri modüllerine geçilmemesi önerilir; aksi hâlde finansal ve resmî kayıtlara erişim sonradan parçalı "
    "biçimde eklenmek zorunda kalır."
)

doc.add_page_break()
add_heading(doc, "Kaynaklar", 1)
sources = [
    ("Türk Medeni Kanunu Madde 84 85 ve 86", "https://www.aile.gov.tr/uploads/ksgm/uploads/pages/mevzuat/turk-medeni-kanunu.pdf"),
    ("Sivil Toplumla İlişkiler Genel Müdürlüğü Üye Kayıt İşlemleri", "https://siviltoplum.gov.tr/ankara/uye-kayit-islemleri"),
    ("Sivil Toplumla İlişkiler Genel Müdürlüğü Dernekler Denetim Rehberi", "https://www.siviltoplum.gov.tr/kurumlar/siviltoplum.gov.tr/Mevzuat/Rehberler/RehberOnaylanan2021Nisan.pdf"),
    ("Karar Evrak Kayıt Demirbaş ve İşletme Hesabı Defterleri Tebliği", "https://siviltoplum.gov.tr/20072-karar-evrak-kayit-demirbas-ve-isletme-hesabi-defterleri"),
    ("Sivil Toplumla İlişkiler Genel Müdürlüğü Derneğin İç Denetimi", "https://siviltoplum.gov.tr/ankara/dernegin-ic-denetimi"),
]
for name, url in sources:
    paragraph = doc.add_paragraph(style="List Bullet")
    paragraph.paragraph_format.space_after = Pt(4)
    add_hyperlink(paragraph, name, url)

add_body(
    doc,
    "Kaynaklar raporun hukuki çerçevesini destekler. Rol adları, ekran izinleri ve onay akışları ise HİSAD projesi "
    "için önerilen yazılım ve iç kontrol tasarımıdır. Uygulama öncesinde dernek tüzüğü ve güncel yönetim kurulu "
    "kararlarıyla karşılaştırılmalıdır.",
    after=0,
)

# Make the first page number hidden while preserving numbering afterward.
first_section = doc.sections[0]
first_section.different_first_page_header_footer = True
first_section.first_page_footer.paragraphs[0].clear()

doc.save(OUTPUT)
print(str(OUTPUT))
