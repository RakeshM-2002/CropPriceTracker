const API_BASE = "https://localhost:5001/api";

function getSelectedDates() {
    const startDate = localStorage.getItem("startDate");
    const endDate = localStorage.getItem("endDate");

    if (!startDate || !endDate) {
        alert("Please select date range on dashboard");
        return null;
    }

    return { startDate, endDate };
}


/* ===== OTP ===== */
async function generateOtp() {
    const mobile = document.getElementById("mobile").value;
    if (!mobile) { document.getElementById("message").innerText = "Enter mobile number"; return; }

    const res = await fetch(`${API_BASE}/auth/generate-otp`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ mobile })
    });

    document.getElementById("message").innerText = res.ok ? "OTP sent" : "Failed to send OTP enter valid Number";
}

/* ========= OTP ========= */
async function verifyOtp() {
    const mobile = document.getElementById("mobile").value;
    const otp = document.getElementById("otp").value;

    if (!otp) {
        document.getElementById("message").innerText = "Enter OTP";
        return;
    }

    const res = await fetch(`${API_BASE}/auth/verify-otp`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({ mobile, otp })
    });

    if (res.ok) {
        const data = await res.json();

        // ✅ STORE userId only
        localStorage.setItem("userId", data.userId);
        localStorage.setItem("mobile", data.mobile);
        localStorage.removeItem("startDate");
        localStorage.removeItem("endDate");

        window.location.href = "/Home/Dashboard";
    } else {
        document.getElementById("message").innerText = "Invalid or expired OTP";
    }
}

function SetData() {
    const startMonth = document.getElementById("startDate").value;
    const endMonth = document.getElementById("endDate").value;

    if (!startMonth || !endMonth) {
        alert("Please select start and end months");
        return;
    }

    const startDate = `${startMonth}-01`;

    const endDate = new Date(
        new Date(endMonth + "-01").getFullYear(),
        new Date(endMonth + "-01").getMonth() + 1,
        0
    ).toISOString().split("T")[0];

    // ✅ STORE globally
    localStorage.setItem("startDate", startDate);
    localStorage.setItem("endDate", endDate);

    alert(`Date range applied: ${startDate} → ${endDate}`);
}



/* ========= ANALYZE ========= */
async function analyze() {
    const cropName = document.getElementById("cropName").value;
    const marketName = document.getElementById("marketName").value;
    const dates = getSelectedDates();

    if (!cropName || !marketName || !dates) {
        alert("Select crop, market and Dates");
        return;
    }

    const res = await fetch(
        `${API_BASE}/crop/analyze?cropName=${cropName}&marketName=${marketName}&startDate=${dates.startDate}&endDate=${dates.endDate}`
    );

    const data = await res.json();

    document.getElementById("analysisResult").innerHTML = `
    <div class="card p-3">
        <h5>📈 ${cropName} - ${marketName}</h5>
        <p><b>Latest Price:</b> ₹${data.latestPrice}</p>
        <p><b>Previous Price:</b> ₹${data.previousPrice}</p>
        <p><b>Change:</b> ₹${data.dayChange} (${data.dayChangePercent}%)</p>
        <p><b>7 Day Avg:</b> ₹${data.average7}</p>
        <p><b>30 Day Avg:</b> ₹${data.average30}</p>
        <p><b>Minimum Price:</b> ₹${data.minimum}</p>
        <p><b>Maximum Price:</b> ₹${data.maximum}</p>
        <p><b>Volatility (Std Dev):</b> ₹${data.standardDeviation}</p>
        <p><b>Trend:</b> ${data.trend}</p>
        <p><b>Summary:</b> ${data.summary}</p>
    </div>
`;

}

/* ========= HISTORY ========= */
async function history() {
    const cropName = document.getElementById("cropName").value;
    const dates = getSelectedDates();
    if (!cropName) {
        alert("Select crop");
        return;
    }

    const res = await fetch(
        `${API_BASE}/crop/history?cropName=${cropName}&startDate=${dates.startDate}&endDate=${dates.endDate}`
    );
    const data = await res.json();

    const tbody = document.getElementById("historyTable");
    tbody.innerHTML = "";

    data.forEach(item => {
        tbody.innerHTML += `
            <tr>
                <td>${item.market}</td>
                <td>${item.modalPrice}</td>
                <td>${item.minPrice}</td>
                <td>${item.maxPrice}</td>
                <td>${new Date(item.createdAt).toLocaleDateString()}</td>
            </tr>`;
    });
}

/* ========= ADD ALERT ========= */
async function addAlert() { 
    const userId = localStorage.getItem("userId");
    const crop = document.getElementById("cropName").value;
    const market = document.getElementById("marketName").value;
    const targetPrice = document.getElementById("targetPrice").value;
    const aboveBelow = document.getElementById("aboveBelow").value;

    if (!userId || !crop || !market || !targetPrice || !aboveBelow) {
        alert("Fill all fields");
        return;
    }

    const res = await fetch(`${API_BASE}/alert/add`, {
        method: "POST",
        headers: { "Content-Type": "application/json" },
        body: JSON.stringify({
            userId,
            crop,
            market,
            targetPrice,
            aboveBelow
        })
    });

    if (res.ok) {
        alert(" Alert added successfully");
    } else {
        alert(" Failed to add alert");
    }
}

/* ========= CHECK ALERTS ========= */
async function checkAlerts() {
    try {
        const userId = localStorage.getItem("userId");
        const dates = getSelectedDates();

        if (!userId) {
            alert("Login required");
            return;
        }

        const res = await fetch(
            `${API_BASE}/alert/my/${userId}?startDate=${dates.startDate}&endDate=${dates.endDate}`
        );

        const data = await res.json();

        if (data.length === 0) {
            document.getElementById("result").innerHTML =
                "<p>No alerts found</p>";
            return;
        }

        let html = "";

        data.forEach(a => {
            html += `
            <div class="card mb-3 p-3">
                <h5>🚨 Price Alert</h5>
                <p><b>Crop:</b> ${a.crop}</p>
                <p><b>Market:</b> ${a.market}</p>
                <p><b>Latest Price:</b> ₹${a.latestPrice}</p>
                <p><b>Target Price:</b> ₹${a.targetPrice}</p>
                <p><b>Status:</b> ${a.condition.toUpperCase()}</p>
                <p><b>Message:</b> ${a.message}</p>
                <small>Created: ${new Date(a.createdAt).toLocaleString()}</small>
            </div>
        `;
        });

        document.getElementById("result").innerHTML = html;
    } catch (error) {
        document.getElementById("result").innerHTML =
            "<p>Error loading alerts</p>";
        console.error(error);
    }
}

/* ========= LOGOUT ========= */
function logout() {
    localStorage.removeItem("userId");
    localStorage.removeItem("mobile");
    window.location.href = "/Home/Login";
}
