import { useEffect, useState } from "react";
import { useParams } from "react-router-dom";
import { getQuizDetails } from "../api/quizApi";

export default function QuizDetails() {

    const { id } = useParams();

    const [quiz, setQuiz] = useState(null);

    useEffect(() => {
        load();
    }, []);

    async function load() {
        const data = await getQuizDetails(id);

        setQuiz(data);
    }

    if (!quiz) {
        return <p>Loading...</p>;
    }

    return (
        <div className="container" style={{ maxWidth: "800px" }}>

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
                                        ${a.isCorrect ? "list-group-item-success fw-bold" : ""}`}
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

                <div className="d-grid mt-4">

                    <button className="btn btn-primary">
                        Open Quiz
                    </button>

                </div>

            </div>

        </div>
    );
}