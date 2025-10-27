import { useState } from "react";
import { Routes, Route } from "react-router-dom";
import "./App.css";
import LoginSingup from "./components/LoginSignup";
import FruitList from "./components/FruitList";
import { AuthProvider } from "./context/authContext";

function App() {
  return (
    <AuthProvider>
      <Routes>
        <Route path="/" element={<FruitList />} />
        <Route path="/Login-Signup" element={<LoginSingup />} />
      </Routes>
    </AuthProvider>
  );
}

export default App;
