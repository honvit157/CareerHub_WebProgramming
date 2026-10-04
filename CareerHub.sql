-- ===============================================================================
-- CƠ SỞ DỮ LIỆU ĐỒ ÁN: QUẢN LÝ THÔNG TIN TUYỂN DỤNG & VIỆC LÀM (CAREERHUB)
-- Chuẩn trình bày theo phong cách bài mẫu của giảng viên
-- ===============================================================================

CREATE DATABASE CareerHub
GO

USE CareerHub
GO

-- 1. BẢNG VAI TRÒ
CREATE TABLE VAITRO
(
	MaVaiTro	INT IDENTITY(1,1)
			CONSTRAINT PK_VAITRO_MaVaiTro PRIMARY KEY,
	TenVaiTro	NVARCHAR(50) NOT NULL,
)

-- 2. BẢNG NGƯỜI DÙNG
CREATE TABLE NGUOIDUNG
(
	MaNguoiDung	INT IDENTITY(1,1)
			CONSTRAINT PK_NGUOIDUNG_MaNguoiDung PRIMARY KEY,
	Email		VARCHAR(100) NOT NULL,
	MatKhau		VARCHAR(255) NOT NULL,
	HoTen		NVARCHAR(50) NOT NULL,
	DienThoai	VARCHAR(15),
	DiaChi		NVARCHAR(100),
	AnhDaiDien	NVARCHAR(255),
	MaVaiTro	INT NOT NULL,
	TrangThai	BIT DEFAULT 1,
	NgayTao		DATETIME DEFAULT GETDATE(),
	CONSTRAINT FK_NGUOIDUNG_MaVaiTro FOREIGN KEY(MaVaiTro)
			REFERENCES VAITRO(MaVaiTro)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
)

-- 3. BẢNG CÔNG TY (HỒ SƠ NHÀ TUYỂN DỤNG)
CREATE TABLE CONGTY
(
	MaCongTy	INT IDENTITY(1,1)
			CONSTRAINT PK_CONGTY_MaCongTy PRIMARY KEY,
	MaNhaTuyenDung	INT NOT NULL,
	TenCongTy	NVARCHAR(100) NOT NULL,
	Logo		NVARCHAR(255),
	Website		VARCHAR(100),
	DiaChi		NVARCHAR(150),
	QuyMo		NVARCHAR(50),
	MoTa		NVARCHAR(MAX),
	DaXacMinh	BIT DEFAULT 0,
	NgayTao		DATETIME DEFAULT GETDATE(),
	CONSTRAINT FK_CONGTY_MaNhaTuyenDung FOREIGN KEY(MaNhaTuyenDung)
			REFERENCES NGUOIDUNG(MaNguoiDung)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
)

-- 4. BẢNG DANH MỤC NGÀNH NGHỀ
CREATE TABLE DANHMUC
(
	MaDanhMuc	CHAR(4)
			CONSTRAINT PK_DANHMUC_MaDanhMuc PRIMARY KEY,
	TenDanhMuc	NVARCHAR(50) NOT NULL,
	MoTa		NVARCHAR(200),
)

-- 5. BẢNG KỸ NĂNG
CREATE TABLE KYNANG
(
	MaKyNang	CHAR(4)
			CONSTRAINT PK_KYNANG_MaKyNang PRIMARY KEY,
	TenKyNang	NVARCHAR(50) NOT NULL,
)

-- 6. BẢNG TIN TUYỂN DỤNG
CREATE TABLE TINTUYENDUNG
(
	MaTin		INT IDENTITY(1,1)
			CONSTRAINT PK_TINTUYENDUNG_MaTin PRIMARY KEY,
	MaCongTy	INT NOT NULL,
	MaDanhMuc	CHAR(4) NOT NULL,
	TieuDe		NVARCHAR(150) NOT NULL,
	MoTaCV		NVARCHAR(MAX),
	YeuCau		NVARCHAR(MAX),
	QuyenLoi	NVARCHAR(MAX),
	DiaChiLamViec	NVARCHAR(150),
	LuongTu		NUMERIC(15,2),
	LuongDen	NUMERIC(15,2),
	LoaiHinh	NVARCHAR(30), -- Full-time, Part-time, Remote, Internship
	KinhNghiem	NVARCHAR(50),
	HanNop		DATETIME,
	TrangThai	NVARCHAR(30) DEFAULT N'Chờ duyệt', -- Chờ duyệt, Đã duyệt, Từ chối, Đã đóng
	SoLuotXem	INT DEFAULT 0,
	NgayDang	DATETIME DEFAULT GETDATE(),
	CONSTRAINT FK_TINTUYENDUNG_MaCongTy FOREIGN KEY(MaCongTy)
			REFERENCES CONGTY(MaCongTy)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
	CONSTRAINT FK_TINTUYENDUNG_MaDanhMuc FOREIGN KEY(MaDanhMuc)
			REFERENCES DANHMUC(MaDanhMuc)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
)

-- 7. BẢNG KỸ NĂNG CỦA TIN TUYỂN DỤNG (BẢNG TRUNG GIAN)
CREATE TABLE KYNANGTIN
(
	MaTin		INT,
	MaKyNang	CHAR(4),
	CONSTRAINT PK_KYNANGTIN PRIMARY KEY(MaTin, MaKyNang),
	CONSTRAINT FK_KYNANGTIN_MaTin FOREIGN KEY(MaTin)
			REFERENCES TINTUYENDUNG(MaTin)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
	CONSTRAINT FK_KYNANGTIN_MaKyNang FOREIGN KEY(MaKyNang)
			REFERENCES KYNANG(MaKyNang)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
)

-- 8. BẢNG HỒ SƠ CV ỨNG VIÊN
CREATE TABLE HOSOCV
(
	MaCV		INT IDENTITY(1,1)
			CONSTRAINT PK_HOSOCV_MaCV PRIMARY KEY,
	MaUngVien	INT NOT NULL,
	TenCV		NVARCHAR(100) NOT NULL,
	DuongDanFile	NVARCHAR(255) NOT NULL,
	TieuSu		NVARCHAR(500),
	MacDinh		BIT DEFAULT 0,
	NgayCapNhat	DATETIME DEFAULT GETDATE(),
	CONSTRAINT FK_HOSOCV_MaUngVien FOREIGN KEY(MaUngVien)
			REFERENCES NGUOIDUNG(MaNguoiDung)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
)

-- 9. BẢNG CHI TIẾT ỨNG TUYỂN
CREATE TABLE UNGTUYEN
(
	MaUngTuyen	INT IDENTITY(1,1)
			CONSTRAINT PK_UNGTUYEN_MaUngTuyen PRIMARY KEY,
	MaTin		INT NOT NULL,
	MaUngVien	INT NOT NULL,
	MaCV		INT NOT NULL,
	ThuGioiThieu	NVARCHAR(1000),
	TrangThai	NVARCHAR(30) DEFAULT N'Đã nộp', -- Đã nộp, Đã duyệt, Mời phỏng vấn, Trúng tuyển, Từ chối
	NgayNop		DATETIME DEFAULT GETDATE(),
	NgayDuyet	DATETIME,
	CONSTRAINT FK_UNGTUYEN_MaTin FOREIGN KEY(MaTin)
			REFERENCES TINTUYENDUNG(MaTin),
	CONSTRAINT FK_UNGTUYEN_MaUngVien FOREIGN KEY(MaUngVien)
			REFERENCES NGUOIDUNG(MaNguoiDung),
	CONSTRAINT FK_UNGTUYEN_MaCV FOREIGN KEY(MaCV)
			REFERENCES HOSOCV(MaCV),
)

-- 10. BẢNG VIỆC LÀM ĐÃ LƯU
CREATE TABLE VIECLAMLUU
(
	MaUngVien	INT,
	MaTin		INT,
	NgayLuu		DATETIME DEFAULT GETDATE(),
	CONSTRAINT PK_VIECLAMLUU PRIMARY KEY(MaUngVien, MaTin),
	CONSTRAINT FK_VIECLAMLUU_MaUngVien FOREIGN KEY(MaUngVien)
			REFERENCES NGUOIDUNG(MaNguoiDung),
	CONSTRAINT FK_VIECLAMLUU_MaTin FOREIGN KEY(MaTin)
			REFERENCES TINTUYENDUNG(MaTin)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
)

-- 11. BẢNG THÔNG BÁO
CREATE TABLE THONGBAO
(
	MaThongBao	INT IDENTITY(1,1)
			CONSTRAINT PK_THONGBAO_MaThongBao PRIMARY KEY,
	MaNguoiDung	INT NOT NULL,
	TieuDe		NVARCHAR(150) NOT NULL,
	NoiDung		NVARCHAR(500) NOT NULL,
	DaDoc		BIT DEFAULT 0,
	NgayTao		DATETIME DEFAULT GETDATE(),
	CONSTRAINT FK_THONGBAO_MaNguoiDung FOREIGN KEY(MaNguoiDung)
			REFERENCES NGUOIDUNG(MaNguoiDung)
			ON DELETE CASCADE
			ON UPDATE CASCADE,
)
GO

-- ===============================================================================
-- DỮ LIỆU MẪU PHONG PHÚ & THỰC TẾ (LARGE SEED DATA)
-- ===============================================================================

-- 1. CHÈN VAI TRÒ
INSERT INTO VAITRO VALUES(N'Quản trị viên');
INSERT INTO VAITRO VALUES(N'Nhà tuyển dụng');
INSERT INTO VAITRO VALUES(N'Ứng viên');

-- 2. CHÈN DANH MỤC NGÀNH NGHỀ
INSERT INTO DANHMUC VALUES('IT01', N'Công nghệ thông tin', N'Lập trình phần mềm, Web, Mobile, AI, Cloud');
INSERT INTO DANHMUC VALUES('MK01', N'Marketing & SEO', N'Digital Marketing, Content Creator, PR, Brand Manager');
INSERT INTO DANHMUC VALUES('TC01', N'Tài chính & Kế toán', N'Kế toán tổng hợp, Kiểm toán, Ngân hàng, Đầu tư');
INSERT INTO DANHMUC VALUES('NS01', N'Nhân sự & Hành chính', N'Tuyển dụng, C&B, Đào tạo nhân sự, Văn phòng');
INSERT INTO DANHMUC VALUES('KD01', N'Kinh doanh & Bán hàng', N'Sales B2B/B2C, Business Development, Telesales');
INSERT INTO DANHMUC VALUES('DH01', N'Thiết kế & Đồ họa', N'UI/UX Design, Đồ họa 2D/3D, Editor Video');
INSERT INTO DANHMUC VALUES('XD01', N'Xây dựng & Kiến trúc', N'Thiết kế kiến trúc, Giám sát công trình, QLDA');

-- 3. CHÈN KỸ NĂNG
INSERT INTO KYNANG VALUES('CS01', N'C# / .NET Core');
INSERT INTO KYNANG VALUES('SQL1', N'SQL Server');
INSERT INTO KYNANG VALUES('REACT', N'ReactJS');
INSERT INTO KYNANG VALUES('JS01', N'JavaScript / TypeScript');
INSERT INTO KYNANG VALUES('PY01', N'Python / Data Science');
INSERT INTO KYNANG VALUES('SEO1', N'SEO Optimization');
INSERT INTO KYNANG VALUES('UIUX', N'UI/UX Design');
INSERT INTO KYNANG VALUES('MISA', N'Kế toán MISA / Fast');
INSERT INTO KYNANG VALUES('ENG1', N'Giao tiếp Tiếng Anh');
INSERT INTO KYNANG VALUES('JAVA', N'Java / Spring Boot');

-- 4. CHÈN NGƯỜI DÙNG (1 Admin, 4 Employers, 5 Candidates)
-- Admin (MaNguoiDung = 1)
INSERT INTO NGUOIDUNG VALUES('admin@careerhub.com', '123456', N'Quản trị viên Hệ thống', '0901234567', N'Hà Nội', '/avatars/admin.jpg', 1, 1, GETDATE());

-- Employers (MaNguoiDung = 2, 3, 4, 5)
INSERT INTO NGUOIDUNG VALUES('hr@fpt.com', '123456', N'Nguyễn Văn Anh (HR FPT)', '0912345678', N'Hà Nội', '/avatars/hr_fpt.jpg', 2, 1, GETDATE());
INSERT INTO NGUOIDUNG VALUES('hr@vng.com', '123456', N'Trần Thị Bảo (HR VNG)', '0923456789', N'Sài Gòn', '/avatars/hr_vng.jpg', 2, 1, GETDATE());
INSERT INTO NGUOIDUNG VALUES('tuyendung@viettel.vn', '123456', N'Lê Minh Cường (HR Viettel)', '0938888888', N'Hà Nội', '/avatars/hr_viettel.jpg', 2, 1, GETDATE());
INSERT INTO NGUOIDUNG VALUES('recruitment@momo.vn', '123456', N'Phạm Hoàng Dung (HR MoMo)', '0949999999', N'Sài Gòn', '/avatars/hr_momo.jpg', 2, 1, GETDATE());

-- Candidates (MaNguoiDung = 6, 7, 8, 9, 10)
INSERT INTO NGUOIDUNG VALUES('nam.le@gmail.com', '123456', N'Lê Hoàng Nam', '0934567890', N'Hà Nội', '/avatars/cand_nam.jpg', 3, 1, GETDATE());
INSERT INTO NGUOIDUNG VALUES('lan.pham@gmail.com', '123456', N'Phạm Thu Lan', '0945678901', N'Đà Nẵng', '/avatars/cand_lan.jpg', 3, 1, GETDATE());
INSERT INTO NGUOIDUNG VALUES('tuan.nguyen@gmail.com', '123456', N'Nguyễn Anh Tuấn', '0956789012', N'Sài Gòn', '/avatars/cand_tuan.jpg', 3, 1, GETDATE());
INSERT INTO NGUOIDUNG VALUES('hoa.tran@gmail.com', '123456', N'Trần Thị Phương Hoa', '0967890123', N'Hà Nội', '/avatars/cand_hoa.jpg', 3, 1, GETDATE());
INSERT INTO NGUOIDUNG VALUES('duc.vu@gmail.com', '123456', N'Vũ Minh Đức', '0978901234', N'Cần Thơ', '/avatars/cand_duc.jpg', 3, 1, GETDATE());

-- 5. CHÈN CÔNG TY (4 Công ty)
INSERT INTO CONGTY VALUES(2, N'Công ty Cổ phần FPT Software', '/images/companies/fpt.png', 'https://fpt-software.com', N'Nam Từ Liêm, Hà Nội', N'10000+ nhân viên', N'Tập đoàn CNTT hàng đầu Việt Nam cung cấp dịch vụ xuất khẩu phần mềm.', 1, GETDATE());
INSERT INTO CONGTY VALUES(3, N'Công ty Cổ phần VNG', '/images/companies/vng.png', 'https://vng.com.vn', N'Quận 7, Sài Gòn', N'3000+ nhân viên', N'Công ty công nghệ kỳ lân sở hữu hệ sinh thái Zalo, Zing, ZaloPay.', 1, GETDATE());
INSERT INTO CONGTY VALUES(4, N'Tập đoàn Công nghiệp - Viễn thông Quân đội Viettel', '/images/companies/viettel.png', 'https://viettel.com.vn', N'Cầu Giấy, Hà Nội', N'50000+ nhân viên', N'Tập đoàn viễn thông và công nghệ hàng đầu khu vực.', 1, GETDATE());
INSERT INTO CONGTY VALUES(5, N'Công ty Cổ phần Dịch vụ Di động Trực tuyến MoMo', '/images/companies/momo.png', 'https://momo.vn', N'Quận 1, Sài Gòn', N'2000+ nhân viên', N'Ví điện tử MoMo - Siêu ứng dụng thanh toán số 1 Việt Nam.', 1, GETDATE());

-- 6. CHÈN TIN TUYỂN DỤNG (6 Bài đăng)
INSERT INTO TINTUYENDUNG VALUES(1, 'IT01', N'Lập trình viên Senior .NET Core (ASP.NET / SQL)', 
 N'Tham gia thiết kế kiến trúc Microservices và RESTful API cho dự án Enterprise quốc tế.', 
 N'Trên 3 năm kinh nghiệm với C#, ASP.NET Core, EF Core, SQL Server. Tiếng Anh giao tiếp tốt.', 
 N'Lương tháng 13, Bảo hiểm FPT Care gia đình, Thưởng dự án hấp dẫn, Du lịch hàng năm.', 
 N'Hà Nội', 22000000, 38000000, N'Full-time', N'Senior', '12/31/2026', N'Đã duyệt', 120, GETDATE());

INSERT INTO TINTUYENDUNG VALUES(1, 'IT01', N'Thực tập sinh Lập trình Web C# / ASP.NET', 
 N'Được đào tạo bài bản bởi các chuyên gia Senior .NET. Tham gia phát triển hệ thống quản lý nội bộ.', 
 N'Sinh viên năm cuối hoặc mới tốt nghiệp ngành CNTT. Nắm vững tư duy C# và SQL Server cơ bản.', 
 N'Trợ cấp thực tập 6.000.000 VNĐ/tháng, Cơ hội ký hợp đồng chính thức sau 3 tháng.', 
 N'Hà Nội', 6000000, 8000000, N'Internship', N'Entry', '11/30/2026', N'Đã duyệt', 250, GETDATE());

INSERT INTO TINTUYENDUNG VALUES(2, 'IT01', N'Frontend Developer (ReactJS / TypeScript)', 
 N'Phát triển giao diện web tốc độ cao cho siêu ứng dụng Zalo Web.', 
 N'Có 2-4 năm kinh nghiệm ReactJS, Redux/Zustand, TailwindCSS. Thành thạo HTML5/CSS3.', 
 N'Trang bị Macbook Pro M2, Thưởng KPIs theo quý, Gói khám sức khỏe định kỳ VIP.', 
 N'Sài Gòn', 18000000, 32000000, N'Full-time', N'Mid', '12/15/2026', N'Đã duyệt', 95, GETDATE());

INSERT INTO TINTUYENDUNG VALUES(3, 'IT01', N'Chuyên viên An toàn Thông tin & Bảo mật (SOC)', 
 N'Giám sát và ứng cứu sự cố an ninh mạng cho hạ tầng mạng viễn thông quốc gia.', 
 N'Tốt nghiệp ngành An toàn thông tin / CNTT. Có chứng chỉ CEH, CISSP hoặc tương đương.', 
 N'Môi trường làm việc Quân đội chuyên nghiệp, Phụ cấp thâm niên, Lương thưởng cạnh tranh.', 
 N'Hà Nội', 25000000, 45000000, N'Full-time', N'Senior', '12/20/2026', N'Đã duyệt', 60, GETDATE());

INSERT INTO TINTUYENDUNG VALUES(4, 'MK01', N'Chuyên viên Digital Marketing / Performance Ads', 
 N'Lên chiến dịch chạy quảng cáo Facebook Ads, Google Ads thúc đẩy người dùng ví MoMo.', 
 N'Ít nhất 2 năm kinh nghiệm chạy Ads ngân sách lớn. Có tư duy phân tích dữ liệu Data-driven.', 
 N'Thưởng phần trăm theo doanh số chiến dịch, Môi trường làm việc trẻ trung sáng tạo.', 
 N'Sài Gòn', 15000000, 25000000, N'Full-time', N'Mid', '11/25/2026', N'Đã duyệt', 180, GETDATE());

INSERT INTO TINTUYENDUNG VALUES(4, 'TC01', N'Kế toán Viên Tổng hợp (Chi phí & Thuế)', 
 N'Hạch toán sổ sách kế toán, kiểm tra chứng từ thu chi và lập báo cáo thuế định kỳ.', 
 N'Tốt nghiệp chuyên ngành Kế toán / Tài chính. Thành thạo phần mềm MISA và Excel nâng cao.', 
 N'Lương cứng + Phụ cấp ăn trưa, Đóng BHXH đầy đủ theo Luật lao động.', 
 N'Sài Gòn', 12000000, 18000000, N'Full-time', N'Mid', '12/10/2026', N'Chờ duyệt', 30, GETDATE());

-- 7. CHÈN KỸ NĂNG CHO TIN TUYỂN DỤNG
INSERT INTO KYNANGTIN VALUES(1, 'CS01'); -- Tin 1 cần C#
INSERT INTO KYNANGTIN VALUES(1, 'SQL1'); -- Tin 1 cần SQL Server
INSERT INTO KYNANGTIN VALUES(1, 'ENG1'); -- Tin 1 cần Tiếng Anh

INSERT INTO KYNANGTIN VALUES(2, 'CS01'); -- Tin 2 cần C#
INSERT INTO KYNANGTIN VALUES(2, 'SQL1'); -- Tin 2 cần SQL Server

INSERT INTO KYNANGTIN VALUES(3, 'REACT'); -- Tin 3 cần ReactJS
INSERT INTO KYNANGTIN VALUES(3, 'JS01');  -- Tin 3 cần JS

INSERT INTO KYNANGTIN VALUES(5, 'SEO1');  -- Tin 5 cần SEO
INSERT INTO KYNANGTIN VALUES(6, 'MISA');  -- Tin 6 cần MISA

-- 8. CHÈN HỒ SƠ CV CỦA CÁC ỨNG VIÊN (7 CV)
INSERT INTO HOSOCV VALUES(6, N'CV Lập Trình Viên .NET Senior - Lê Hoàng Nam', '/uploads/cvs/cv_lehoangnam_net.pdf', N'Kỹ sư phần mềm có 4 năm kinh nghiệm phát triển ứng dụng web C# .NET Core và SQL Server.', 1, GETDATE());
INSERT INTO HOSOCV VALUES(6, N'CV Tiếng Anh .NET Developer - Le Hoang Nam', '/uploads/cvs/cv_lehoangnam_en.pdf', N'Senior Software Engineer specializing in C# and Microservices architecture.', 0, GETDATE());

INSERT INTO HOSOCV VALUES(7, N'CV Frontend Developer - Phạm Thu Lan', '/uploads/cvs/cv_phamthulan_frontend.pdf', N'Chuyên viên thiết kế và phát triển giao diện Web với ReactJS, TailwindCSS.', 1, GETDATE());

INSERT INTO HOSOCV VALUES(8, N'CV Chuyên viên An ninh mạng - Nguyễn Anh Tuấn', '/uploads/cvs/cv_nguyenanhtuan_soc.pdf', N'Chuyên gia An toàn thông tin có chứng chỉ CEH, đam mê bảo mật hệ thống.', 1, GETDATE());

INSERT INTO HOSOCV VALUES(9, N'CV Digital Marketing Manager - Trần Thị Phương Hoa', '/uploads/cvs/cv_tranphuonghoa_marketing.pdf', N'Chuyên viên chạy Ads và xây dựng chiến lược truyền thông thương hiệu.', 1, GETDATE());

INSERT INTO HOSOCV VALUES(10, N'CV Kế toán Tổng hợp - Vũ Minh Đức', '/uploads/cvs/cv_vuminhduc_ketoan.pdf', N'Kế toán viên có 3 năm kinh nghiệm làm việc trên phần mềm MISA và FAST.', 1, GETDATE());

-- 9. CHÈN LƯỢT ỨNG TUYỂN (6 Đơn nộp với các trạng thái khác nhau)
INSERT INTO UNGTUYEN VALUES(1, 6, 1, N'Kính gửi Bộ phận Tuyển dụng FPT Software, tôi mong muốn ứng tuyển vị trí Senior .NET Developer.', N'Đã duyệt', '10/01/2026', '10/02/2026');
INSERT INTO UNGTUYEN VALUES(2, 6, 1, N'Em xin ứng tuyển vị trí Thực tập sinh .NET để tích lũy thêm kinh nghiệm ạ.', N'Đã nộp', '10/03/2026', NULL);
INSERT INTO UNGTUYEN VALUES(3, 7, 3, N'Chào anh chị HR VNG, em gửi CV ứng tuyển vị trí ReactJS Frontend Developer.', N'Mời phỏng vấn', '10/02/2026', '10/04/2026');
INSERT INTO UNGTUYEN VALUES(4, 8, 4, N'Tôi muốn nộp hồ sơ ứng tuyển vị trí Chuyên viên SOC tại Viettel Telecom.', N'Trúng tuyển', '09/25/2026', '09/28/2026');
INSERT INTO UNGTUYEN VALUES(5, 9, 5, N'Chào chị HR MoMo, em gửi hồ sơ ứng tuyển vị trí Performance Ads.', N'Đã nộp', '10/04/2026', NULL);
INSERT INTO UNGTUYEN VALUES(6, 10, 6, N'Em xin nộp CV ứng tuyển vị trí Kế toán tổng hợp ạ.', N'Từ chối', '09/20/2026', '09/22/2026');

-- 10. CHÈN VIỆC LÀM ĐÃ LƯU
INSERT INTO VIECLAMLUU VALUES(6, 1, GETDATE());
INSERT INTO VIECLAMLUU VALUES(6, 3, GETDATE());
INSERT INTO VIECLAMLUU VALUES(7, 3, GETDATE());
INSERT INTO VIECLAMLUU VALUES(8, 4, GETDATE());
INSERT INTO VIECLAMLUU VALUES(9, 5, GETDATE());

-- 11. CHÈN THÔNG BÁO HỆ THỐNG
INSERT INTO THONGBAO VALUES(6, N'Hồ sơ được chấp nhận', N'Công ty FPT Software đã duyệt hồ sơ ứng tuyển vị trí Senior .NET Developer của bạn.', 1, GETDATE());
INSERT INTO THONGBAO VALUES(7, N'Lịch phỏng vấn mới', N'Công ty VNG trân trọng mời bạn tham gia phỏng vấn vị trí Frontend Developer vào ngày 10/10/2026.', 0, GETDATE());
INSERT INTO THONGBAO VALUES(8, N'Chúc mừng trúng tuyển!', N'Tập đoàn Viettel xin chúc mừng bạn đã trúng tuyển vị trí Chuyên viên SOC.', 1, GETDATE());
INSERT INTO THONGBAO VALUES(9, N'Ứng tuyển thành công', N'Đơn ứng tuyển của bạn vào MoMo đã được gửi tới nhà tuyển dụng.', 0, GETDATE());
GO