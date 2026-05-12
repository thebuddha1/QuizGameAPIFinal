const API_BASE = "/api/player";

export async function joinQuiz(data) {

    const token = localStorage.getItem("token");

    const headers = {
        "Content-Type": "application/json"
    };

    if (token) {
        headers.Authorization = `Bearer ${token}`;
    }

    const res = await fetch(`${API_BASE}/join`, {
        method: "POST",
        headers,
        body: JSON.stringify(data)
    });

    if (!res.ok) {

        const text = await res.text();

        throw new Error(text || "Failed to join quiz");
    }

    return await res.json();
}

export async function submitAnswer(data) {

    const res = await fetch(`${API_BASE}/submit`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });

    if (!res.ok) {

        const text = await res.text();

        throw new Error(text || "Failed to submit answer");
    }
}