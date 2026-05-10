import { Link, Outlet } from "react-router-dom";

export default function Layout() {

    const token = localStorage.getItem("token");

    return (
        <div className="d-flex flex-column min-vh-100">

            <header>

                <nav className="navbar navbar-expand-lg navbar-light bg-light border-bottom">

                    <div className="container">

                        <Link
                            className="navbar-brand fw-bold"
                            to="/"
                        >
                            QuizGame
                        </Link>

                        <button
                            className="navbar-toggler"
                            type="button"
                            data-bs-toggle="collapse"
                            data-bs-target="#navbarContent"
                        >
                            <span className="navbar-toggler-icon"></span>
                        </button>

                        <div
                            className="collapse navbar-collapse"
                            id="navbarContent"
                        >

                            <ul className="navbar-nav ms-auto">

                                <li className="nav-item">
                                    <Link className="nav-link" to="/">
                                        Quizzes
                                    </Link>
                                </li>

                                {token ? (
                                    <>
                                        <li className="nav-item">
                                            <Link
                                                className="nav-link"
                                                to="/my-quizzes"
                                            >
                                                My Quizzes
                                            </Link>
                                        </li>

                                        <li className="nav-item">
                                            <Link
                                                className="nav-link"
                                                to="/create"
                                            >
                                                Create Quiz
                                            </Link>
                                        </li>

                                        <li className="nav-item">

                                            <button
                                                className="nav-link btn btn-link"
                                                onClick={() => {

                                                    localStorage.removeItem("token");

                                                    window.location.href = "/";
                                                }}
                                            >
                                                Logout
                                            </button>

                                        </li>
                                    </>
                                ) : (
                                    <>
                                        <li className="nav-item">

                                            <Link
                                                className="nav-link"
                                                to="/login"
                                            >
                                                Login
                                            </Link>

                                        </li>

                                        <li className="nav-item">

                                            <Link
                                                className="nav-link"
                                                to="/register"
                                            >
                                                Register
                                            </Link>

                                        </li>
                                    </>
                                )}

                            </ul>

                        </div>

                    </div>

                </nav>

            </header>

            <main
                className="container d-flex justify-content-center align-items-center flex-grow-1 py-4"
            >

                <div className="w-100 text-center">

                    <Outlet />

                </div>

            </main>

            <footer className="border-top text-muted mt-auto">

                <div className="container text-center py-3">

                    © 2026 - QuizGame

                </div>

            </footer>

        </div>
    );
}