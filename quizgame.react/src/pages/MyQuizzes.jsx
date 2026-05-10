import { useEffect, useState } from "react";
import { getMyQuizzes, deleteQuiz } from "../api/quizApi";
import { Link } from "react-router-dom";

export default function MyQuizzes() {

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

    return (
        <div className="container" style={{ maxWidth: "700px" }}>

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

                        <div className="d-flex gap-2">

                            <Link
                                to={`/quiz/${q.id}`}
                                className="btn btn-primary btn-sm"
                            >
                                Open
                            </Link>

                            <button
                                className="btn btn-danger btn-sm"
                                onClick={() => handleDelete(q.id)}
                            >
                                Delete
                            </button>

                        </div>

                    </div>

                ))}

            </div>

        </div>
    );
}