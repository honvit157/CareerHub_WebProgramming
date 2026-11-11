
document.addEventListener("DOMContentLoaded", function () {
    const form = document.getElementById("jobSearchForm");
    const jobList = document.getElementById("jobList");
    const pagination = document.getElementById("jobPagination");
    const jobCount = document.getElementById("jobCount");

    if (!form || !jobList || !pagination) {
        console.warn("Missing AJAX elements in Jobs/Index.cshtml");
        return;
    }

    let currentRequest = 0;
    let controller = null;

    function createElement(tag, className = "", text = null) {
        const element = document.createElement(tag);

        if (className) {
            element.className = className;
        }

        if (text !== null && text !== undefined) {
            element.textContent = String(text);
        }

        return element;
    }

    function formatSalary(value) {
        if (value === null || value === undefined) {
            return "Negotiable";
        }

        return new Intl.NumberFormat("vi-VN").format(value);
    }

    function formatDate(value) {
        if (!value) return "Not specified";

        const parts = String(value).substring(0, 10).split("-");

        if (parts.length !== 3) {
            return String(value);
        }

        return `${parts[2]}/${parts[1]}/${parts[0]}`;
    }

    function addIconText(parent, iconClass, value) {
        const span = createElement(
            "span",
            "text-truncate me-3"
        );

        const icon = createElement(
            "i",
            iconClass + " text-primary me-2"
        );

        span.appendChild(icon);
        span.appendChild(
            document.createTextNode(value)
        );

        parent.appendChild(span);
    }

    function createJobCard(job) {
        const card = createElement(
            "div",
            "job-item p-4 mb-4"
        );

        const row = createElement("div", "row g-4");

        const left = createElement(
            "div",
            "col-sm-12 col-md-8 d-flex align-items-center"
        );

        const img = createElement(
            "img",
            "flex-shrink-0 img-fluid border rounded"
        );

        const logo = job.logo
            ? String(job.logo).split(/[\\/]/).pop()
            : "com-logo-1.jpg";

        img.src = "/Template/img/" + encodeURIComponent(logo);
        img.alt = "Company logo";
        img.style.width = "80px";
        img.style.height = "80px";
        img.style.objectFit = "contain";

        img.onerror = function () {
            this.onerror = null;
            this.src = "/Template/img/com-logo-1.jpg";
        };

        const info = createElement(
            "div",
            "text-start ps-4"
        );

        const title = createElement("h5", "mb-2");

        const titleLink = createElement(
            "a",
            "text-dark",
            job.tieuDe || "Untitled Job"
        );

        titleLink.href =
            "/Jobs/Details/" + encodeURIComponent(job.maTin);

        title.appendChild(titleLink);

        const company = createElement(
            "span",
            "d-block text-muted mb-2",
            job.tenCongTy || "Company not specified"
        );

        info.appendChild(title);
        info.appendChild(company);

        addIconText(
            info,
            "fa fa-map-marker-alt",
            job.diaChiLamViec || "Not specified"
        );

        addIconText(
            info,
            "far fa-clock",
            job.loaiHinh || "Not specified"
        );

        let salaryText = "Negotiable";

        if (job.luongTu != null && job.luongDen != null) {
            salaryText =
                formatSalary(job.luongTu) +
                " - " +
                formatSalary(job.luongDen) +
                " VND";
        } else if (job.luongTu != null) {
            salaryText = "From " +
                formatSalary(job.luongTu) + " VND";
        } else if (job.luongDen != null) {
            salaryText = "Up to " +
                formatSalary(job.luongDen) + " VND";
        }

        addIconText(
            info,
            "far fa-money-bill-alt",
            salaryText
        );

        left.appendChild(img);
        left.appendChild(info);

        const right = createElement(
            "div",
            "col-sm-12 col-md-4 d-flex flex-column " +
            "align-items-start align-items-md-end justify-content-center"
        );

        const button = createElement(
            "a",
            "btn btn-primary mb-3",
            "View Details"
        );

        button.href =
            "/Jobs/Details/" + encodeURIComponent(job.maTin);

        const deadline = createElement("small", "text-muted");

        const calendar = createElement(
            "i",
            "far fa-calendar-alt text-primary me-2"
        );

        deadline.appendChild(calendar);
        deadline.appendChild(
            document.createTextNode(
                "Deadline: " + formatDate(job.hanNop)
            )
        );

        right.appendChild(button);
        right.appendChild(deadline);

        row.appendChild(left);
        row.appendChild(right);

        card.appendChild(row);

        return card;
    }

    function renderJobs(jobs) {
        jobList.replaceChildren();

        if (!Array.isArray(jobs) || jobs.length === 0) {
            jobList.appendChild(
                createElement(
                    "div",
                    "alert alert-info",
                    "No jobs found matching your search."
                )
            );
            return;
        }

        jobs.forEach(function (job) {
            jobList.appendChild(createJobCard(job));
        });
    }

    function createPageButton(label, page, disabled, active) {
        const li = createElement("li", "page-item");

        if (disabled) {
            li.classList.add("disabled");
        }

        if (active) {
            li.classList.add("active");
        }

        const button = createElement(
            "button",
            "page-link",
            label
        );

        button.type = "button";
        button.disabled = disabled;

        if (active) {
            button.setAttribute("aria-current", "page");
        }

        button.addEventListener("click", function () {
            if (!disabled && !active) {
                loadJobs(page);
            }
        });

        li.appendChild(button);

        return li;
    }

    function renderPagination(data) {
        pagination.replaceChildren();

        if (!data.totalPages || data.totalPages <= 1) {
            return;
        }

        const nav = createElement("nav");
        nav.setAttribute("aria-label", "Job pagination");

        const ul = createElement(
            "ul",
            "pagination justify-content-center mt-4"
        );

        ul.appendChild(
            createPageButton(
                "Previous",
                data.page - 1,
                data.page <= 1,
                false
            )
        );

        for (let i = 1; i <= data.totalPages; i++) {
            ul.appendChild(
                createPageButton(
                    String(i),
                    i,
                    false,
                    i === data.page
                )
            );
        }

        ul.appendChild(
            createPageButton(
                "Next",
                data.page + 1,
                data.page >= data.totalPages,
                false
            )
        );

        nav.appendChild(ul);
        pagination.appendChild(nav);
    }

    async function loadJobs(page = 1, updateHistory = true) {
        const requestId = ++currentRequest;

        if (controller) {
            controller.abort();
        }

        controller = new AbortController();

        const params = new URLSearchParams(new FormData(form));
        params.set("page", String(page));

        const queryString = params.toString();

        jobList.replaceChildren(
            createElement(
                "div",
                "text-center p-4",
                "Loading jobs..."
            )
        );

        try {
            const response = await fetch(
                "/api/JobsApi?" + queryString,
                {
                    method: "GET",
                    headers: {
                        "Accept": "application/json"
                    },
                    signal: controller.signal
                }
            );

            if (!response.ok) {
                throw new Error(
                    "API returned status " + response.status
                );
            }

            const data = await response.json();

            if (requestId !== currentRequest) {
                return;
            }

            renderJobs(data.jobs);
            renderPagination(data);

            if (jobCount) {
                jobCount.textContent =
                    data.totalJobs + " job(s) found";
            }

            if (updateHistory) {
                window.history.pushState(
                    {},
                    "",
                    window.location.pathname + "?" + queryString
                );
            }

        } catch (error) {
            if (error.name === "AbortError") {
                return;
            }

            if (requestId !== currentRequest) {
                return;
            }

            console.error("AJAX error:", error);

            jobList.replaceChildren(
                createElement(
                    "div",
                    "alert alert-danger",
                    "Unable to load jobs. Please try again."
                )
            );
        }
    }

    function restoreFormFromUrl() {
        const params = new URLSearchParams(
            window.location.search
        );

        const elements = Array.from(form.elements);

        elements.forEach(function (element) {
            if (!element.name) return;

            const values = params.getAll(element.name);

            if (
                element.type === "checkbox" ||
                element.type === "radio"
            ) {
                element.checked = values.includes(element.value);
            } else if (
                element.tagName === "SELECT" ||
                element.type === "text" ||
                element.type === "number"
            ) {
                if (params.has(element.name)) {
                    element.value = params.get(element.name);
                }
            }
        });
    }

    form.addEventListener("submit", function (event) {
        event.preventDefault();
        loadJobs(1);
    });

    // Handle pagination links rendered by Razor initially.
    pagination.addEventListener("click", function (event) {
        const target = event.target;

        if (!(target instanceof Element)) return;

        const link = target.closest("a.page-link");

        if (!link || !pagination.contains(link)) return;

        event.preventDefault();

        const url = new URL(
            link.getAttribute("href"),
            window.location.href
        );

        const page = Number(
            url.searchParams.get("page") || 1
        );

        loadJobs(page);
    });

    window.addEventListener("popstate", function () {
        restoreFormFromUrl();

        const params = new URLSearchParams(
            window.location.search
        );

        const page = Number(params.get("page") || 1);

        loadJobs(page, false);
    });
});
