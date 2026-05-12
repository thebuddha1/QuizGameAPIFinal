import { useEffect, useState } from "react";

import {
    getMyQuizzes,
    deleteQuiz
} from "../api/quizApi";

import { openSession } from "../api/sessionApi";

import {
    Link,
    useNavigate
} from "react-router-dom";

export default function MyQuizzes() {

    const navigate = useNavigate();

    const [quizzes, setQuizzes] = useState([]);

    useEffect(() => {
        load();
    }, []);

    async function load() {

        const token = localStorage.getItem("token");

        const data = await getMyQuizzes(token);

        setQuizzes(data);
    }

    async function handleDelete(id) {

        const token = localStorage.getItem("token");

        await deleteQuiz(id, token);

        load();
    }

    async function handleOpen(id) {

        try {

            const sessionId =
                await openSession(id);

            navigate(`/session/${sessionId}`);

        }
        catch (err) {

            alert(err.message);
        }
    }

    return (
        <div
            className="container"
            style={{ maxWidth: "700px" }}
        >

            <h2 className="text-center mb-4">
                My Quizzes
            </h2>

            <div className="d-flex flex-column gap-3">

                {quizzes.map(q => (

                    <div
                        key={q.id}
                        className="card shadow-sm p-3 d-flex flex-row justify-content-between align-items-center"
                    >

                        <Link
                            to={`/quiz/${q.id}`}
                            className="fw-bold text-decoration-none"
                        >
                            {q.title}
                        </Link>

                        <div className="d-flex gap-2 flex-wrap">

                            {q.hasActiveSession ? (

                                <button
                                    className="btn btn-warning btn-sm"
                                    onClick={() =>
                                        navigate(`/session/${q.activeSessionId}`)
                                    }
                                >
                                    Return to Session
                                </button>

                            ) : (

                                <button
                                    className="btn btn-primary btn-sm"
                                    onClick={() => handleOpen(q.id)}
                                >
                                    Open new quiz session
                                </button>

                            )}

                            <button
                                className="btn btn-secondary btn-sm"
                                disabled={q.hasActiveSession}
                                onClick={() =>
                                    navigate(`/edit/${q.id}`)
                                }
                            >
                                Edit quiz
                            </button>

                            <button
                                className="btn btn-danger btn-sm"
                                disabled={q.hasActiveSession}
                                onClick={() => handleDelete(q.id)}
                            >
                                Delete quiz
                            </button>

                        </div>

                    </div>

                ))}

            </div>

        </div>
    );
}