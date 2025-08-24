// Backend'den gelen veri yapılarını burada tanımlayacağız.
export interface User {
  fullName: string;
  email: string;
  roles: string[];
}

export interface Category {
    categoryID: number;
    categoryName: string;
    description?: string;
    parentCategoryID?: number;
    isActive: boolean;
}