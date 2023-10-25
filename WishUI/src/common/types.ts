export interface UserClaim {
    id: string;
    userName: string;

    email: string | null;
    name: string | null;
    pictureUrl: string | null;
    userType: number;
}

export interface LoginResult {
    token: string;
    user: UserClaim;
    isNew: boolean;
}

export interface WishCrudModel {
    id: string | null;
    name: string;
    subWishes: SubWishCrudModel[];
    wishOptions: WishOptionCrudModel[];
    entityState: number;
}

export interface SubWishCrudModel {
    id: string | null;
    name: string;
    entityState: number;
}

export interface WishOptionCrudModel {
    id: string | null;
    name: string,
    value: number;
}

export interface Response {
    success: boolean;
    message: string | null;
}

export interface ResponseResult<T> {
    success: boolean;
    message: string | null;
    result: T | null;
}

export interface Wish {
    id: string;
    name: string;

    userId: string;

    subWishes: SubWish[];
    wishOptions: WishOption[];

    entityState: number;
}

export interface SubWish {
    id: string;
    name: string;

    wishId: string;

    entityState: number;
}

export interface WishOption {
    id: string;
    name: string;
    value: number;

    wishId: string;

    entityState: number;
}

export interface DailyWish {
    id: string;
    pkDate: Date | string;

    wishId: string;
    wish: Wish | null;

    userId: string;

    selectedOptionId: string;
    selectedOption: WishOption | null;

    remark: string | null;

    dailySubWishes: DailySubWish[];
}

export interface DailySubWish {
    id: string;

    dailyWishId: string;

    subWishId: string;
    subWish: SubWish | null;

    selectedOptionId: string;
    selectedOption: WishOption | null;

    remark: string | null;
}

export interface DailyWishInsertModel {
    //#region Not from backend
    locked: boolean;
    wish: WishCrudModel;
    //#endregion

    pkDate: Date | string;

    wishId: string;
    optionId: string;
    remark: string | null;

    subWishOptions: SubWishOption[];
}

export interface SubWishOption {
    //#region Not from backend
    subWish: SubWishCrudModel;
    //#endregion

    subWishId: string;
    optionId: string;
    remark: string | null;
}


export interface DailyWishSummary {
    wishId: string;
    name: string;
    count: number;
    sum: number;
    mean: number;

    remarks: RemarkSummary[];
    dailySubWishes: DailySubWishSummary[];
    wishOptions: WishOptionCrudModel[];
}

export interface DailySubWishSummary {
    subWishId: string;
    name: string;
    count: number;
    sum: number;
    mean: number;

    //remarks: RemarkSummary[];
}

export interface RemarkSummary {
    pkDate: string;
    remark: string;
}