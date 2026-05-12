import { useEffect, useState } from "react";
import { getQuizzes } from "../api/api";
import { useNavigate } from "react-router-dom";

export default function Home() {

    const [active, setActive] = useState([]);
    const [other, setOther] = useState([]);
    const navigate = useNavigate();

    async function load() {

        try {

            const data = await getQuizzes();

            setActive(data.activeQuizzes || []);
            setOther(data.otherQuizzes || []);

        } catch (err) {

            console.error(err);
        }
    }

    useEffect(() => {

        load();

    }, []);

    return (
        <div style={{ maxWidth: "700px", margin: "auto" }}>

            <h2 className="text-center mb-4">
                Active Quizzes
            </h2>

            {active.length === 0 ? (
                <p className="text-muted">
                    No active quizzes
                </p>
            ) : (
                <div className="d-flex flex-column gap-3 mb-5">

                    {active.map(q => (
                        <div
                            key={q.id}
                            className="card shadow-sm p-3 d-flex flex-row justify-content-between"
                        >
                            <strong>{q.title}</strong>

                            <button
                                className="btn btn-primary btn-sm"
                                onClick={() => navigate(`/join/${q.id}`)}
                            >
                                Join
                            </button>

                        </div>
                    ))}

                </div>
            )}

            <h2 className="text-center mb-4">
                Other Quizzes
            </h2>

            {other.length === 0 ? (
                <p className="text-muted">
                    No other quizzes
                </p>
            ) : (
                <div className="d-flex flex-column gap-3">

                    {other.map(q => (
                        <div
                            key={q.id}
                            className="card shadow-sm p-3"
                        >
                            {q.title}
                        </div>
                    ))}

                </div>
            )}

        </div>
    );
}