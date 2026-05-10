const API_BASE = "https://localhost:7001/api";

export async function getQuizzes() {
    const res = await fetch(`${API_BASE}/quiz`);

    if (!res.ok) throw new Error("Failed to fetch quizzes");

    return await res.json();
}