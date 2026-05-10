import { useState } from "react";
import { useNavigate } from "react-router-dom";
import { createQuiz } from "../api/quizApi";

export default function CreateQuiz() {
    const navigate = useNavigate();

    const [title, setTitle] = useState("");

    const [questions, setQuestions] = useState([
        {
            text: "",
            timeLimitSeconds: 30,
            order: 0,
            answers: []
        }
    ]);

    function addQuestion() {
        setQuestions([
            ...questions,
            {
                text: "",
                timeLimitSeconds: 30,
                order: questions.length,
                answers: []
            }
        ]);
    }

    function removeQuestion(index) {
        setQuestions(questions.filter((_, i) => i !== index));
    }

    function updateQuestion(index, field, value) {
        const updated = [...questions];
        updated[index][field] = value;
        setQuestions(updated);
    }

    function addAnswer(qIndex) {
        const updated = [...questions];

        updated[qIndex].answers.push({
            text: "",
            isCorrect: false
        });

        setQuestions(updated);
    }

    function removeAnswer(qIndex, aIndex) {
        const updated = [...questions];

        updated[qIndex].answers =
            updated[qIndex].answers.filter((_, i) => i !== aIndex);

        setQuestions(updated);
    }

    function updateAnswer(qIndex, aIndex, field, value) {
        const updated = [...questions];

        updated[qIndex].answers[aIndex][field] = value;

        setQuestions(updated);
    }

    async function handleSubmit(e) {

        e.preventDefault();

        if (!title.trim()) {
            alert("Quiz title cannot be empty");
            return;
        }

        if (questions.length === 0) {
            alert("At least one question is required");
            return;
        }

        for (let i = 0; i < questions.length; i++) {

            const q = questions[i];

            if (!q.text.trim()) {
                alert(`Question ${i + 1} cannot be empty`);
                return;
            }

            if (q.timeLimitSeconds <= 0) {
                alert(`Question ${i + 1} must have a valid time limit`);
                return;
            }

            if (q.answers.length < 2) {
                alert(`Question ${i + 1} needs at least 2 answers`);
                return;
            }

            const hasCorrect = q.answers.some(a => a.isCorrect);

            if (!hasCorrect) {
                alert(`Question ${i + 1} must have at least one correct answer`);
                return;
            }

            for (let j = 0; j < q.answers.length; j++) {

                const a = q.answers[j];

                if (!a.text.trim()) {
                    alert(`Answer ${j + 1} in Question ${i + 1} cannot be empty`);
                    return;
                }
            }
        }

        const payload = {
            title: title.trim(),
            questions: questions.map((q, index) => ({
                ...q,
                text: q.text.trim(),
                order: index,
                answers: q.answers.map(a => ({
                    ...a,
                    text: a.text.trim()
                }))
            }))
        };

        try {

            const token = localStorage.getItem("token");

            const createdQuizId = await createQuiz(payload, token);

            navigate(`/quiz/${createdQuizId}`);
        }
        catch (err) {

            alert(err.message);
        }
    }
   

    return (
        <div className="container" style={{ maxWidth: "800px" }}>

            <div className="card shadow-sm p-4">

                <h2 className="text-center mb-4">
                    Create Quiz
                </h2>

                <form onSubmit={handleSubmit}>

                    <div className="mb-4">
                        <label className="form-label">
                            Title
                        </label>

                        <input
                            className="form-control"
                            value={title}
                            onChange={(e) => setTitle(e.target.value)}
                        />
                    </div>

                    <h4 className="mb-3">
                        Questions
                    </h4>

                    <div className="d-flex flex-column gap-3">

                        {questions.map((q, qIndex) => (

                            <div
                                key={qIndex}
                                className="card p-3 shadow-sm"
                            >

                                <div className="mb-2">
                                    <input
                                        className="form-control"
                                        placeholder="Question text"
                                        value={q.text}
                                        onChange={(e) =>
                                            updateQuestion(
                                                qIndex,
                                                "text",
                                                e.target.value
                                            )
                                        }
                                    />
                                </div>

                                <div className="mb-2">
                                    <input
                                        className="form-control"
                                        type="number"
                                        placeholder="Time limit"
                                        value={q.timeLimitSeconds}
                                        onChange={(e) =>
                                            updateQuestion(
                                                qIndex,
                                                "timeLimitSeconds",
                                                Number(e.target.value)
                                            )
                                        }
                                    />
                                </div>

                                <div className="d-flex flex-column gap-2 mb-2">

                                    {q.answers.map((a, aIndex) => (

                                        <div
                                            key={aIndex}
                                            className="d-flex align-items-center gap-2"
                                        >

                                            <input
                                                className="form-control"
                                                placeholder="Answer"
                                                value={a.text}
                                                onChange={(e) =>
                                                    updateAnswer(
                                                        qIndex,
                                                        aIndex,
                                                        "text",
                                                        e.target.value
                                                    )
                                                }
                                            />

                                            <input
                                                type="checkbox"
                                                checked={a.isCorrect}
                                                onChange={(e) =>
                                                    updateAnswer(
                                                        qIndex,
                                                        aIndex,
                                                        "isCorrect",
                                                        e.target.checked
                                                    )
                                                }
                                            />

                                            <button
                                                type="button"
                                                className="btn btn-outline-danger btn-sm"
                                                onClick={() =>
                                                    removeAnswer(qIndex, aIndex)
                                                }
                                            >
                                                ✕
                                            </button>

                                        </div>

                                    ))}

                                </div>

                                <div className="d-flex gap-2">

                                    <button
                                        type="button"
                                        className="btn btn-outline-primary btn-sm"
                                        onClick={() => addAnswer(qIndex)}
                                    >
                                        + Answer
                                    </button>

                                    <button
                                        type="button"
                                        className="btn btn-outline-danger btn-sm"
                                        onClick={() => removeQuestion(qIndex)}
                                    >
                                        - Remove Question
                                    </button>

                                </div>

                            </div>

                        ))}

                    </div>

                    <div className="d-flex justify-content-center mt-3">

                        <button
                            type="button"
                            className="btn btn-outline-primary"
                            onClick={addQuestion}
                        >
                            + Add Question
                        </button>

                    </div>

                    <div className="d-grid mt-4">

                        <button
                            type="submit"
                            className="btn btn-primary"
                        >
                            Create Quiz
                        </button>

                    </div>

                </form>

            </div>

        </div>
    );
}