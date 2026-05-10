import { useEffect, useState } from "react";

import {
    endSession,
    getSession,
    lockQuestion,
    nextQuestion,
    startSession
} from "../api/sessionApi";

import { submitAnswer } from "../api/playerApi";

import {
    useNavigate,
    useParams,
    useSearchParams
} from "react-router-dom";

export default function ManageSession() {

    const navigate = useNavigate();

    const { sessionId } = useParams();

    const [searchParams] = useSearchParams();

    const participantId = searchParams.get("participantId");

    const [session, setSession] = useState(null);

    async function loadSession() {

        const data = await getSession(
            sessionId,
            participantId
        );

        setSession(data);
    }

    useEffect(() => {

        loadSession();

    }, []);

    if (!session) {
        return <p>Loading...</p>;
    }

    async function handleStart() {

        await startSession(sessionId);

        loadSession();
    }

    async function handleLock() {

        await lockQuestion(sessionId);

        loadSession();
    }

    async function handleNext() {

        await nextQuestion(sessionId);

        loadSession();
    }

    async function handleEnd() {

        await endSession(sessionId);

        navigate("/my-quizzes");
    }

    async function handleSubmit(answerId) {

        await submitAnswer({
            sessionId: session.sessionId,
            participantId: session.participantId,
            questionId: session.currentQuestion.id,
            answerOptionId: answerId
        });

        loadSession();
    }

    return (
        <div
            className="container"
            style={{ maxWidth: "800px" }}
        >

            <div className="card shadow-sm p-4">

                <h2 className="text-center mb-3">
                    {session.title}
                </h2>

                <div className="text-center mb-4">

                    <span className="fw-bold">
                        Participants:
                    </span>

                    <span className="badge bg-secondary ms-2">
                        {session.participantsCount}
                    </span>

                </div>

                {!session.isStarted && !session.isFinished && (

                    <div className="text-center">

                        <p className="mb-3">
                            Waiting to start...
                        </p>

                        {session.isOwner && (

                            <button
                                className="btn btn-primary"
                                onClick={handleStart}
                            >
                                Start Quiz
                            </button>

                        )}

                    </div>
                )}

                {session.isFinished && (

                    <div className="text-center">

                        <h3 className="mb-3">
                            Quiz Finished
                        </h3>

                        {session.isOwner ? (

                            <button
                                className="btn btn-primary"
                                onClick={handleEnd}
                            >
                                End Quiz
                            </button>

                        ) : (

                            <button
                                className="btn btn-primary"
                                onClick={() => navigate("/")}
                            >
                                Quit Quiz
                            </button>

                        )}

                    </div>
                )}

                {session.isStarted &&
                    !session.isFinished &&
                    session.currentQuestion && (

                        <>
                            <div className="card p-3 shadow-sm mb-3">

                                <h5 className="mb-3">
                                    {session.currentQuestion.text}
                                </h5>

                                <div className="list-group mb-3">

                                    {session.currentQuestion.answers.map(a => (

                                        <label
                                            key={a.id}
                                            className={`list-group-item d-flex justify-content-between align-items-center ${(session.hasAnswered || session.isLocked) &&
                                                    a.isCorrect
                                                    ? "list-group-item-success fw-bold"
                                                    : ""
                                                }`}
                                        >

                                            <div>

                                                <input
                                                    type="radio"
                                                    className="form-check-input me-2"
                                                    checked={session.selectedAnswerId === a.id}
                                                    disabled={
                                                        session.hasAnswered ||
                                                        session.isLocked ||
                                                        session.isOwner
                                                    }
                                                    onChange={() =>
                                                        handleSubmit(a.id)
                                                    }
                                                />

                                                {a.text}

                                            </div>

                                            <div>

                                                {session.hasAnswered &&
                                                    session.selectedAnswerId === a.id && (

                                                        <span className="badge bg-primary">
                                                            Your answer
                                                        </span>

                                                    )}

                                                {(session.hasAnswered || session.isLocked) &&
                                                    a.isCorrect && (

                                                        <span className="badge bg-success">
                                                            Correct
                                                        </span>

                                                    )}

                                            </div>

                                        </label>

                                    ))}

                                </div>

                                {!session.isOwner &&
                                    session.hasAnswered && (

                                        <div className={`alert ${session.isCorrect
                                                ? "alert-success"
                                                : "alert-danger"
                                            } mt-3 text-center`}>

                                            {session.isCorrect
                                                ? "Correct!"
                                                : "Wrong answer"}

                                        </div>
                                    )}

                                {!session.isOwner &&
                                    session.isLocked &&
                                    !session.hasAnswered && (

                                        <div className="alert alert-warning mt-3 text-center">
                                            Time is up!
                                        </div>
                                    )}

                            </div>

                            {session.isOwner && (

                                <div className="text-center">

                                    {!session.isLocked ? (

                                        <button
                                            className="btn btn-warning"
                                            onClick={handleLock}
                                        >
                                            Lock Question
                                        </button>

                                    ) : (

                                        <button
                                            className="btn btn-primary"
                                            onClick={handleNext}
                                        >
                                            Next Question
                                        </button>

                                    )}

                                </div>
                            )}

                        </>
                    )}

            </div>

        </div>
    );
}