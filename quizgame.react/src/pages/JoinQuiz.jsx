import { useState } from "react";
import { useNavigate, useParams } from "react-router-dom";
import { joinQuiz } from "../api/playerApi";

export default function JoinQuiz() {

    const navigate = useNavigate();

    const { quizId } = useParams();

    const [pinCode, setPinCode] = useState("");

    const [nickname, setNickname] = useState("");

    const [error, setError] = useState("");

    async function handleSubmit(e) {

        e.preventDefault();

        setError("");

        try {

            const result = await joinQuiz({
                quizId: parseInt(quizId),
                pinCode,
                nickname
            });

            navigate(
                `/session/${result.sessionId}?participantId=${result.participantId}`
            );
        }
        catch (err) {

            if (
                err.message.includes("started")
            ) {

                setError(
                    "This quiz has already started. Joining is closed."
                );

                return;
            }

            if (
                err.message.includes("owner")
            ) {

                setError(
                    "Quiz owners cannot join their own quiz as participants."
                );

                return;
            }

            if (
                err.message.includes("Invalid PIN")
            ) {

                setError(
                    "Incorrect PIN code."
                );

                return;
            }

            setError(err.message);
        }
    }

    return (
        <div
            className="container"
            style={{ maxWidth: "400px" }}
        >

            <div className="card shadow-sm p-4">

                <h2 className="text-center mb-4">
                    Join Quiz
                </h2>

                <form onSubmit={handleSubmit}>

                    <div className="mb-3">

                        <label className="form-label">
                            PIN Code
                        </label>

                        <input
                            className="form-control"
                            value={pinCode}
                            onChange={(e) => setPinCode(e.target.value)}
                        />

                    </div>

                    <div className="mb-3">

                        <label className="form-label">
                            Nickname (optional)
                        </label>

                        <input
                            className="form-control"
                            value={nickname}
                            onChange={(e) => setNickname(e.target.value)}
                        />

                    </div>

                    <div className="d-grid mt-3">

                        <button
                            type="submit"
                            className="btn btn-primary"
                        >
                            Join
                        </button>

                    </div>

                </form>

                {error && (

                    <div className="mt-3 text-center">

                        <div className="text-danger mb-3">
                            {error}
                        </div>

                        {error.includes("started") && (

                            <button
                                className="btn btn-secondary"
                                onClick={() => navigate("/")}
                            >
                                Return Home
                            </button>

                        )}

                    </div>
                )}

            </div>

        </div>
    );
}