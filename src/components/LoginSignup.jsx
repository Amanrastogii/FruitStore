import { useState } from "react";
import axios from "axios";
import { useNavigate } from "react-router-dom";
import { useAuth } from "../context/authContext";

const LoginSingup = () => {
  const [register, setRegister] = useState(true);
  const [firstName, setFirstName] = useState("");
  const [lastName, setLastName] = useState("");
  const [email, setEmail] = useState("");
  const [password, setPassword] = useState("");
  const [role, setRole] = useState("");

  const navigate = useNavigate();

  const { login } = useAuth();

  const handleSubmit = async (e) => {
    e.preventDefault();

    const parts = email.split("@");
    var part = parts[1].split(".")[0] === "admin" ? "Admin" : "Candidate";
    setRole(part);

    try {
      if (register) {
        console.log(part);
        const res = await axios.post(
          "https://fruitstore-mi21.onrender.com/api/AccountApi/register",
          { email: email, password: password, role: part }
        );
        setRegister(false);
        console.log(res.data);
      } else {
        console.log(register);
        const res = await axios.post(
          "https://fruitstore-mi21.onrender.com/api/AccountApi/login",
          { email, password }
        );
        login(part);
        navigate("/");
        console.log(res.data);
      }
    } catch (err) {
      console.log("error while login and signup : ", err);
    }
  };

  return (
    <div className="flex justify-center items-center min-h-screen bg-gray-900">
      <form
        className="bg-gray-800 p-8 rounded-2xl shadow-lg w-full max-w-sm space-y-5"
        onSubmit={handleSubmit}
      >
        <h1 className="text-2xl font-semibold text-center text-white">
          {register ? "Register" : "Login"}
        </h1>

        {register && (
          <>
            <div className="flex flex-col">
              <label htmlFor="firstName" className="text-gray-300 mb-1 text-sm">
                Enter First Name
              </label>
              <input
                id="firstName"
                type="text"
                value={firstName}
                onChange={(e) => {
                  setFirstName(e.target.value);
                }}
                placeholder="Enter Your First Name"
                className="p-3 rounded-lg bg-gray-700 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
              />
            </div>
            <div className="flex flex-col">
              <label htmlFor="lastName" className="text-gray-300 mb-1 text-sm">
                Enter Last Name
              </label>
              <input
                id="lastName"
                type="text"
                value={lastName}
                onChange={(e) => setLastName(e.target.value)}
                placeholder="Enter Your Last Name"
                className="p-3 rounded-lg bg-gray-700 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
              />
            </div>
          </>
        )}
        {/* Email Field */}
        <div className="flex flex-col">
          <label htmlFor="Email" className="text-gray-300 mb-1 text-sm">
            Enter Email
          </label>
          <input
            id="Email"
            type="email"
            value={email}
            onChange={(e) => {
              setEmail(e.target.value);
            }}
            placeholder="Enter Your Email"
            className="p-3 rounded-lg bg-gray-700 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />
        </div>

        {/* Password Field */}
        <div className="flex flex-col">
          <label htmlFor="Password" className="text-gray-300 mb-1 text-sm">
            Enter Password
          </label>
          <input
            id="Password"
            type="password"
            value={password}
            onChange={(e) => setPassword(e.target.value)}
            placeholder="Enter Your Password"
            className="p-3 rounded-lg bg-gray-700 text-white placeholder-gray-400 focus:outline-none focus:ring-2 focus:ring-indigo-500"
          />
        </div>

        <div>
          <p>
            {register ? "Already Registered? " : "Havent registered yet? "}

            <strong
              className="text-white"
              onClick={() => {
                console.log("clicked");
                setRegister(!register);
              }}
            >
              {register ? "Login" : "Register"}
            </strong>
          </p>
        </div>

        {/* Submit Button */}
        <button
          type="submit"
          className="w-full bg-indigo-600 hover:bg-indigo-700 text-white font-medium py-3 rounded-lg transition-all duration-200"
        >
          Login
        </button>
      </form>
    </div>
  );
};

export default LoginSingup;
