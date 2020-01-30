import { ITagData } from 'src/app/feature/project/reusable/inputs/tag-multiselect/tag-multiselect.interface';

export interface IProfileEditPage {
    member: IProfile;
    title: string;
    url: string;
}
export interface IProfile {
    id: string;
    firstName: string;
    lastName: string;
    phone: string;
    department: string;
    photo: string;
    photoId: string;
    email: string;
    profileUrl: string;
    mediaRootId: string;
    newMedia: string;
    memberNotifierSettings: string;
    tags: Array<ITagData>;
    availableTags: Array<ITagData>;
}
