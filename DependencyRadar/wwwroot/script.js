const packageList = document.getElementById("package-list");
const addPackageButton = document.getElementById("add-package");

console.log("Dependency Radar JS loaded");

addPackageButton.addEventListener("click", () => {
    const packageRow = document.createElement("div");

    packageRow.classList.add("package-row");

    packageRow.innerHTML = `
        <input
            type="text"
            name="package"
            placeholder="Package name"
            required
        >

        <input
            type="text"
            name="version"
            placeholder="Version"
            required
        >

        <button type="button" class="remove-package">
            ✕
        </button>
    `;

    packageList.appendChild(packageRow);
});

// Remove a package row
packageList.addEventListener("click", (event) => {
    if (event.target.classList.contains("remove-package")) {
        event.target.closest(".package-row").remove();
    }
});


const scanForm = document.getElementById("scan-form");
const resultsSection = document.getElementById("results");
const scanButton = document.getElementById("scan-button");

scanForm.addEventListener("submit", async (event) => {
    event.preventDefault();
    resultsSection.replaceChildren();

    const packageRows = document.querySelectorAll(".package-row");

    const packages = Array.from(packageRows).map((row) => {
        const inputs = row.querySelectorAll("input");

        return {
            name: inputs[0].value.trim(),
            version: inputs[1].value.trim()
        };
    });

    // Show loading message
    scanButton.disabled = true;
    scanButton.textContent = "Scanning...";
    resultsSection.textContent = "Checking dependencies...";

    try {
        const response = await fetch("/api/scans", {
            method: "POST",
            headers: {
                "Content-Type": "application/json"
            },
            body: JSON.stringify({ packages })
        });

        const data = await response.json();

        if (!response.ok) {
            throw new Error(
                data.detail || data.error || "An error occurred during the scan."
            );
        }

        console.log("Scan results:", data);
        displayResults(data);

    } catch (error) {
        console.error(error);
        resultsSection.textContent = error.message;

    } finally {
        scanButton.disabled = false;
        scanButton.textContent = "Scan dependencies";
    }
});


function displayResults(results) {
    resultsSection.replaceChildren();

    results.forEach((result) => {
        const card = document.createElement("div");
        card.classList.add("result-card");

        // Package header
        const header = document.createElement("div");
        header.classList.add("result-header");

        const title = document.createElement("h3");
        title.textContent = result.package;

        const status = document.createElement("span");
        const isVulnerable = result.vulnerabilityCount > 0;

        status.classList.add(
            "status",
            isVulnerable ? "vulnerable" : "secure"
        );

        status.textContent = isVulnerable
            ? "Vulnerable"
            : "No known vulnerabilities";

        header.append(title, status);
        card.appendChild(header);

        // Scanned version
        const version = document.createElement("p");
        version.classList.add("version");
        version.textContent = `Version scanned: ${result.version}`;
        card.appendChild(version);

        // Vulnerability count
        const count = document.createElement("p");
        count.classList.add(
            "vulnerability-count",
            isVulnerable ? "has-vulnerabilities" : "no-vulnerabilities"
        );

        count.textContent = isVulnerable
            ? `${result.vulnerabilityCount} vulnerability detected`
            : "No known vulnerabilities found";

        card.appendChild(count);

        // Vulnerability details
        result.vulnerabilities.forEach((vuln) => {
            const details = document.createElement("div");
            details.classList.add("vulnerability-details");

            const id = document.createElement("strong");
            id.textContent = vuln.id;

            const summary = document.createElement("p");
            summary.textContent = vuln.summary || "No description available.";

            const link = document.createElement("a");
            link.href = `https://osv.dev/vulnerability/${encodeURIComponent(vuln.id)}`;
            link.textContent = "View on OSV ↗";
            link.target = "_blank";
            link.rel = "noopener noreferrer";

            details.append(id, summary, link);
            card.appendChild(details);
        });

        // Fixed versions
        const fixedVersion = document.createElement("div");
        fixedVersion.classList.add("fixed-version");

        const label = document.createElement("span");
        label.textContent = "Recommended fixed version";

        const value = document.createElement("strong");
        value.textContent = result.fixedVersions.length > 0
            ? result.fixedVersions.join(", ")
            : "No fixed version available";

        fixedVersion.append(label, value);
        card.appendChild(fixedVersion);

        resultsSection.appendChild(card);
    });
}
