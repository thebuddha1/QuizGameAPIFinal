const API_BASE = "/api/quiz";

export async function getQuizzes() {
    const res = await fetch(API_BASE);

    if (!res.ok) {
        throw new Error("Failed to fetch quizzes");
    }

    return await res.json();
}

export async function getMyQuizzes(token) {
    const res = await fetch(`${API_BASE}/my`, {
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    if (!res.ok) {
        throw new Error("Failed to fetch my quizzes");
    }

    return await res.json();
}

export async function getQuizDetails(id) {
    const res = await fetch(`${API_BASE}/${id}`);

    if (!res.ok) {
        throw new Error("Failed to fetch quiz details");
    }

    return await res.json();
}

export async function createQuiz(data, token) {

    const res = await fetch(API_BASE, {
        method: "POST",
        headers: {
            "Content-Type": "application/json",
            Authorization: `Bearer ${token}`
        },
        body: JSON.stringify(data)
    });

    if (!res.ok) {

        const errorText = await res.text();

        throw new Error(errorText);
    }

    return await res.json();
}

export async function deleteQuiz(id, token) {
    const res = await fetch(`${API_BASE}/${id}`, {
        method: "DELETE",
        headers: {
            Authorization: `Bearer ${token}`
        }
    });

    if (!res.ok) {
        throw new Error("Failed to delete quiz");
    }
}