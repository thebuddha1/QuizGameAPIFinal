import { BrowserRouter, Routes, Route } from "react-router-dom";

import Layout from "./components/Layout";

import Home from "./pages/Home";
import CreateQuiz from "./pages/CreateQuiz";
import QuizDetails from "./pages/QuizDetails";
import MyQuizzes from "./pages/MyQuizzes";
import Login from "./pages/Login";
import Register from "./pages/Register";
import JoinQuiz from "./pages/JoinQuiz";
import ManageSession from "./pages/ManageSession";

function App() {

    return (

        <BrowserRouter>

            <Routes>

                <Route path="/" element={<Layout />}>

                    <Route index element={<Home />} />

                    <Route path="create" element={<CreateQuiz />} />

                    <Route path="my-quizzes" element={<MyQuizzes />} />

                    <Route path="quiz/:id" element={<QuizDetails />} />

                    <Route path="login" element={<Login />} />

                    <Route path="register" element={<Register />} />

                    <Route path="join/:quizId" element={<JoinQuiz />} />

                    <Route
                        path="session/:sessionId"
                        element={<ManageSession />}
                    />

                </Route>

            </Routes>

        </BrowserRouter>
    );
}

export default App;