import { useEffect, useState } from "react";
import { useAuth } from "../context/authContext";
import FruitCard from "./FruitCard";
import axios from "axios";

const FruitList = () => {
  const { role } = useAuth();
  const [fruitlist, setFruitList] = useState([]);
  const [name, setName] = useState("");
  const [desc, setDesc] = useState("");
  const [price, setPrice] = useState("");
  const [stock, setStock] = useState("");
  const [image, setImage] = useState(null);

  const fetchFruits = async () => {
    try {
      const res = await axios.get(
        "https://fruitstore-mi21.onrender.com/api/FruitApi"
      );
      console.log("data while fetching in dashboad : ", res.data);
      setFruitList(res.data);
    } catch (e) {
      console.log("error while fetching dashboard data");
    }
  };

  useEffect(() => {
    fetchFruits();
  }, []);

  const handleDelete = async (id) => {
    await axios.delete(
      `https://fruitstore-mi21.onrender.com/api/FruitApi/${id}`
    );
    setFruitList((prev) => {
      prev.filter((fruit) => fruit.id !== id);
    });
    fetchFruits();
  };

  return (
    <div className="min-h-screen bg-gray-50 p-4 sm:p-6 md:p-8">
            {/* Header Section */}     {" "}
      <div className="text-center mb-6 sm:mb-8 md:mb-10 px-2">
               {" "}
        <h1 className="text-2xl sm:text-3xl md:text-4xl font-bold text-gray-800 mb-2">
                    Hello {role}, Fruit Dashboard 🍓        {" "}
        </h1>
               {" "}
        <p className="text-sm sm:text-base text-gray-500">
                    Handpicked fresh fruits delivered to your door        {" "}
        </p>
             {" "}
      </div>
            {/* Admin Form */}     {" "}
      {role === "Admin" && (
        <form
          onSubmit={async (e) => {
            e.preventDefault();
            const formData = new FormData();
            formData.append("Name", name);
            formData.append("Description", desc);
            formData.append("Price", parseFloat(price));
            formData.append("Stock", parseInt(stock));
            formData.append("Image", image);

            try {
              const res = await axios.post(
                "https://fruitstore-mi21.onrender.com/api/FruitApi",
                formData,
                { headers: { "Content-Type": "multipart/form-data" } }
              );
              console.log("Product uploaded:", res.data);
              setFruitList((prev) => [...prev, res.data]); // Reset form

              setName("");
              setDesc("");
              setPrice("");
              setStock("");
              setImage(null);
            } catch (err) {
              console.error("Upload failed:", err);
            }
          }}
          className="w-full max-w-lg mx-auto bg-white p-4 sm:p-6 md:p-8 rounded-xl shadow-lg space-y-4 sm:space-y-5 mb-8 sm:mb-10 md:mb-12"
        >
                   {" "}
          <h2 className="text-xl sm:text-2xl md:text-3xl font-bold text-gray-800 text-center mb-2 sm:mb-4">
                        Add New Fruit 🍎          {" "}
          </h2>
                    {/* Product Name */}         {" "}
          <div className="flex flex-col">
                       {" "}
            <label className="mb-1 sm:mb-2 font-medium text-gray-700 text-sm sm:text-base">
                            Product Name            {" "}
            </label>
                       {" "}
            <input
              type="text"
              value={name}
              onChange={(e) => setName(e.target.value)}
              placeholder="Enter product name"
              required
              className="border border-gray-300 p-2 sm:p-3 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm sm:text-base"
            />
                     {" "}
          </div>
                    {/* Product Description */}         {" "}
          <div className="flex flex-col">
                       {" "}
            <label className="mb-1 sm:mb-2 font-medium text-gray-700 text-sm sm:text-base">
                            Description            {" "}
            </label>
                       {" "}
            <textarea
              value={desc}
              onChange={(e) => setDesc(e.target.value)}
              placeholder="Enter product description"
              required
              rows="3"
              className="border border-gray-300 p-2 sm:p-3 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 resize-none text-sm sm:text-base"
            />
                     {" "}
          </div>
                   {" "}
          {/* Price & Stock - Stacked on mobile, side-by-side on larger screens */}
                   {" "}
          <div className="flex flex-col sm:flex-row gap-3 sm:gap-4">
                       {" "}
            <div className="flex-1 flex flex-col">
                           {" "}
              <label className="mb-1 sm:mb-2 font-medium text-gray-700 text-sm sm:text-base">
                                Price              {" "}
              </label>
                           {" "}
              <input
                type="number"
                value={price}
                onChange={(e) => setPrice(e.target.value)}
                placeholder="₹0"
                required
                min="0"
                step="0.01"
                className="border border-gray-300 p-2 sm:p-3 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm sm:text-base"
              />
                         {" "}
            </div>
                       {" "}
            <div className="flex-1 flex flex-col">
                           {" "}
              <label className="mb-1 sm:mb-2 font-medium text-gray-700 text-sm sm:text-base">
                                Stock              {" "}
              </label>
                           {" "}
              <input
                type="number"
                value={stock}
                onChange={(e) => setStock(e.target.value)}
                placeholder="0"
                required
                min="0"
                className="border border-gray-300 p-2 sm:p-3 rounded-lg focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm sm:text-base"
              />
                         {" "}
            </div>
                     {" "}
          </div>
                    {/* Image Upload */}         {" "}
          <div className="flex flex-col">
                       {" "}
            <label className="mb-1 sm:mb-2 font-medium text-gray-700 text-sm sm:text-base">
                            Upload Image            {" "}
            </label>
                       {" "}
            <input
              type="file"
              accept="image/*"
              onChange={(e) => setImage(e.target.files[0])}
              required
              className="border border-gray-300 p-2 rounded-lg cursor-pointer focus:outline-none focus:ring-2 focus:ring-blue-400 text-sm sm:text-base file:mr-2 sm:file:mr-4 file:py-1 sm:file:py-2 file:px-2 sm:file:px-4 file:rounded-md file:border-0 file:text-xs sm:file:text-sm file:font-semibold file:bg-blue-50 file:text-blue-700 hover:file:bg-blue-100"
            />
                     {" "}
          </div>
                    {/* Submit Button */}         {" "}
          <button
            type="submit"
            className="w-full bg-blue-500 text-white p-2.5 sm:p-3 rounded-lg font-semibold hover:bg-blue-600 active:bg-blue-700 transition-colors duration-300 text-sm sm:text-base shadow-md hover:shadow-lg"
          >
                        Add Fruit          {" "}
          </button>
                 {" "}
        </form>
      )}
            {/* Fruit Cards Grid - Responsive */}     {" "}
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-4 sm:gap-5 md:gap-6 max-w-7xl mx-auto">
               {" "}
        {Array.isArray(fruitlist) &&
          fruitlist.map((fruit) => (
            <FruitCard
              key={fruit.id}
              id={fruit.id}
              name={fruit.name}
              description={fruit.description}
              price={fruit.price}
              image={fruit.imagePath || fruit.image}
              onDelete={handleDelete}
            />
          ))}
             {" "}
      </div>
         {" "}
    </div>
  );
};

export default FruitList;
