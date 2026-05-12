const API_BASE = "/api/session";

function authHeader() {

    const token = localStorage.getItem("token");

    return token
        ? { Authorization: `Bearer ${token}` }
        : {};
}

export async function openSession(quizId) {

    const res = await fetch(
        `${API_BASE}/open/${quizId}`,
        {
            method: "POST",
            headers: {
                ...authHeader()
            }
        }
    );

    if (!res.ok) {

        const text = await res.text();

        throw new Error(text || "Failed to open session");
    }

    return await res.json();
}

export async function getSession(
    sessionId,
    participantId
) {

    let url = `${API_BASE}/${sessionId}`;

    if (participantId) {
        url += `?participantId=${participantId}`;
    }

    const res = await fetch(url, {
        headers: {
            ...authHeader()
        }
    });

    if (!res.ok) {

        const text = await res.text();

        throw new Error(text || "Failed to load session");
    }

    return await res.json();
}

export async function startSession(sessionId) {

    const res = await fetch(
        `${API_BASE}/${sessionId}/start`,
        {
            method: "POST",
            headers: {
                ...authHeader()
            }
        }
    );

    if (!res.ok) {

        const text = await res.text();

        throw new Error(text || "Failed to start session");
    }
}

export async function lockQuestion(sessionId) {

    const res = await fetch(
        `${API_BASE}/${sessionId}/lock`,
        {
            method: "POST",
            headers: {
                ...authHeader()
            }
        }
    );

    if (!res.ok) {

        const text = await res.text();

        throw new Error(text || "Failed to lock question");
    }
}

export async function nextQuestion(sessionId) {

    const res = await fetch(
        `${API_BASE}/${sessionId}/next`,
        {
            method: "POST",
            headers: {
                ...authHeader()
            }
        }
    );

    if (!res.ok) {

        const text = await res.text();

        throw new Error(text || "Failed to go next");
    }
}

export async function endSession(sessionId) {

    const res = await fetch(
        `${API_BASE}/${sessionId}/end`,
        {
            method: "POST",
            headers: {
                ...authHeader()
            }
        }
    );

    if (!res.ok) {

        const text = await res.text();

        throw new Error(text || "Failed to end session");
    }
}