import { Trash } from "lucide-react";
import { useAuth } from "../context/authContext";

const FruitCard = ({ id, image, name, description, price, onDelete }) => {
  const { role } = useAuth();

  if (image === null) {
    image =
      "https://hebbkx1anhila5yf.public.blob.vercel-storage.com/attachments/gen-images/public/fresh-red-strawberries-XGgKkRI9Nq6yEOReUW1rykCeyMpXSQ.jpg";
  }
  return (
    <div className="bg-white rounded-xl shadow-md overflow-hidden max-w-xs border border-gray-200 hover:shadow-lg transition-shadow fruitCard">
      {role === "Admin" && (
        <Trash className="delete" onClick={() => onDelete(id)} />
      )}

      <img
        src={`http://fruitstore-mi21.onrender.com/${image}`}
        alt="Fresh Strawberries"
        className="w-full h-48 object-cover"
      />
      <div className="p-4">
        <h1 className="text-lg font-semibold text-gray-800">{name}</h1>
        <p className="text-gray-500 text-sm mt-1">{description}</p>
        <div className="flex items-center justify-between mt-4">
          <p className="text-green-600 font-bold text-lg">{`$${price}`}</p>
          <button className="bg-green-600 text-white text-sm font-medium py-1.5 px-3 rounded-lg hover:bg-green-700 transition">
            Add to Cart
          </button>
        </div>
      </div>
    </div>
  );
};

export default FruitCard;
