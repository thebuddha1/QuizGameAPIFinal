import { useState } from "react";
import { useNavigate } from "react-router-dom";
//import { register } from "../api/authApi";
import { register, login } from "../api/authApi";

export default function Register() {

    const navigate = useNavigate();

    const [name, setName] = useState("");

    const [email, setEmail] = useState("");

    const [password, setPassword] = useState("");

    const [error, setError] = useState("");

    async function handleSubmit(e) {

        e.preventDefault();

        setError("");

        try {

            await register({
                name,
                email,
                password
            });

            const result = await login({
                email,
                password
            });

            localStorage.setItem("token", result.authToken);

            window.location.href = "/";
        }
        catch {

            setError("Registration failed");
        } try {

            await register({
                name,
                email,
                password
            });

            const result = await login({
                email,
                password
            });

            localStorage.setItem("token", result.authToken);

            window.location.href = "/";
        }
        catch {

            setError("Registration failed");
        }
    }

    return (
        <div className="d-flex justify-content-center align-items-center w-100">

            <div
                className="card shadow-sm p-4"
                style={{ maxWidth: "400px", width: "100%" }}
            >

                <h2 className="text-center mb-4">
                    Register
                </h2>

                <form onSubmit={handleSubmit}>

                    <div className="mb-3">

                        <label className="form-label">
                            Name
                        </label>

                        <input
                            className="form-control"
                            value={name}
                            onChange={(e) => setName(e.target.value)}
                        />

                    </div>

                    <div className="mb-3">

                        <label className="form-label">
                            Email
                        </label>

                        <input
                            className="form-control"
                            value={email}
                            onChange={(e) => setEmail(e.target.value)}
                        />

                    </div>

                    <div className="mb-3">

                        <label className="form-label">
                            Password
                        </label>

                        <input
                            type="password"
                            className="form-control"
                            value={password}
                            onChange={(e) => setPassword(e.target.value)}
                        />

                    </div>

                    <div className="d-grid">

                        <button
                            type="submit"
                            className="btn btn-primary"
                        >
                            Register
                        </button>

                    </div>

                </form>

                {error && (
                    <div className="text-danger mt-3 text-center">
                        {error}
                    </div>
                )}

            </div>

        </div>
    );
}