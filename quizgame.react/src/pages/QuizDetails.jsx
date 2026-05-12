import { useEffect, useState } from "react";

import {
    useNavigate,
    useParams
} from "react-router-dom";

import {
    deleteQuiz,
    getQuizDetails
} from "../api/quizApi";

import { openSession } from "../api/sessionApi";

export default function QuizDetails() {

    const navigate = useNavigate();

    const { id } = useParams();

    const [quiz, setQuiz] = useState(null);

    useEffect(() => {
        load();
    }, []);

    async function load() {

        try {

            const data =
                await getQuizDetails(id);

            setQuiz(data);
        }
        catch (err) {

            alert(err.message);
        }
    }

    async function handleOpenQuiz() {

        try {

            const sessionId =
                await openSession(id);

            navigate(`/session/${sessionId}`);
        }
        catch (err) {

            alert(err.message);
        }
    }

    async function handleDelete() {

        try {

            const token =
                localStorage.getItem("token");

            await deleteQuiz(id, token);

            navigate("/my-quizzes");
        }
        catch (err) {

            alert(err.message);
        }
    }

    if (!quiz) {
        return <p>Loading...</p>;
    }

    return (
        <div
            className="container"
            style={{ maxWidth: "800px" }}
        >

            <div className="card shadow-sm p-4">

                <h2 className="text-center mb-3">
                    {quiz.title}
                </h2>

                <div className="text-center mb-4">

                    <span className="fw-bold">
                        PIN:
                    </span>

                    <span className="badge bg-dark fs-6 ms-2">
                        {quiz.pinCode}
                    </span>

                </div>

                <h4 className="mb-3 text-center">
                    Questions
                </h4>

                <div className="d-flex flex-column gap-3">

                    {quiz.questions.map((q, index) => (

                        <div
                            key={index}
                            className="card p-3 shadow-sm"
                        >

                            <div className="fw-bold mb-2">
                                {q.text}
                            </div>

                            <ul className="list-group">

                                {q.answers.map((a, aIndex) => (

                                    <li
                                        key={aIndex}
                                        className={`list-group-item d-flex justify-content-between align-items-center
                                        ${a.isCorrect
                                                ? "list-group-item-success fw-bold"
                                                : ""
                                            }`}
                                    >

                                        {a.text}

                                        {a.isCorrect && (

                                            <span className="badge bg-success">
                                                Correct
                                            </span>

                                        )}

                                    </li>

                                ))}

                            </ul>

                        </div>

                    ))}

                </div>

                <div className="d-flex flex-wrap gap-2 justify-content-center mt-4">

                    {quiz.hasActiveSession ? (

                        <button
                            className="btn btn-warning"
                            onClick={() =>
                                navigate(`/session/${quiz.activeSessionId}`)
                            }
                        >
                            Rejoin Session
                        </button>

                    ) : (

                        <button
                            className="btn btn-primary"
                            onClick={handleOpenQuiz}
                        >
                            Open Quiz
                        </button>

                    )}

                    <button
                        className="btn btn-secondary"
                        disabled={quiz.hasActiveSession}
                        onClick={() =>
                            navigate(`/edit/${quiz.id}`)
                        }
                    >
                        Edit Quiz
                    </button>

                    <button
                        className="btn btn-danger"
                        disabled={quiz.hasActiveSession}
                        onClick={handleDelete}
                    >
                        Delete Quiz
                    </button>

                </div>

            </div>

        </div>
    );
}