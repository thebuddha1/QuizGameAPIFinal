const API_BASE = "/api/player";

export async function joinQuiz(data) {

    const res = await fetch(`${API_BASE}/join`, {
        method: "POST",
        headers: {
            "Content-Type": "application/json"
        },
        body: JSON.stringify(data)
    });

    if (!res.ok) {
        throw new Error("Failed to join quiz");
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
        throw new Error("Failed to submit answer");
    }

    return await res.json();
}