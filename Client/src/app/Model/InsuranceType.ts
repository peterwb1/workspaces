// The API serialises this enum as its name (JsonStringEnumConverter), so the
// values here are the strings that actually travel on the wire.
export enum InsuranceType {
    FullyComprehensive = 'FullyComprehensive',
    ThirdPartyFireAndTheft = 'ThirdPartyFireAndTheft',
    ThirdPartyOnly = 'ThirdPartyOnly'
}
